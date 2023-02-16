using System;

using Microsoft.Extensions.Logging;

using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor.Notifiers
{
    public class NotificationManager
    {
        private readonly ILogger<NotificationManager> _logger;
        private readonly NotificationRepository _repository;

        public NotificationManager(InteractorManager interactorManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NotificationManager>();
            _repository = new NotificationRepository();
            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
        }

        public event EventHandler<NotificationEventArgs>? NewNotificationRequests;

        private void OnFaultedInteractor(object? sender, InteractorFaultedEventArgs args)
        {
            _logger.LogDebug(
                args.Error,
                "Interactor {Interactor} faulted.",
                args.Interactor);

            _repository.RemoveInteractor(args.Interactor);
        }

        private void OnClosedInteractor(object? sender, InteractorClosedEventArgs args)
        {
            _logger.LogDebug(
                "Removing notification requests from {Interactor}.",
                args.Interactor);

            _repository.RemoveInteractor(args.Interactor);
        }

        public void RequestNotification(Interactor notifiable, NotificationRequest notificationRequest)
        {
            _logger.LogInformation(
                "Handling notification request for {Notifiable} with {Message}.",
                notifiable,
                notificationRequest);

            if (notificationRequest.IsAdd)
            {
                if (_repository.AddRequest(notifiable, notificationRequest.Feed))
                    NewNotificationRequests?.Invoke(this, new NotificationEventArgs(notifiable, notificationRequest.Feed));
            }
            else
                _repository.RemoveRequest(notifiable, notificationRequest.Feed);
        }

        internal void ForwardSubscription(Interactor subscriber, SubscriptionRequest subscriptionRequest)
        {
            // Find all the interactors that wish to be notified of subscriptions to this topic.
            var notifiables = _repository.FindNotifiables(subscriptionRequest.Feed);
            if (notifiables == null)
                return;

            subscriber.Metrics.ForwardedSubscriptions[subscriptionRequest.Feed].Inc();

            var forwardedSubscriptionRequest = new ForwardedSubscriptionRequest(
                subscriber.UserForFeed(subscriptionRequest.Feed),
                subscriber.HostForFeed(subscriptionRequest.Feed),
                subscriber.Id,
                subscriptionRequest.Feed,
                subscriptionRequest.Topic,
                subscriptionRequest.IsAdd);

            _logger.LogDebug(
                "Notifying interactors [{Interactors}] of subscription {Message}.",
                string.Join(",", notifiables),
                forwardedSubscriptionRequest);

            // Inform each notifiable interactor of the subscription request.
            foreach (var notifiable in notifiables)
            {
                try
                {
                    notifiable.SendMessage(forwardedSubscriptionRequest);
                }
                catch (Exception error)
                {
                    _logger.LogDebug(
                        error,
                        "Failed to notify {Notifiable} regarding {Message}.",
                        notifiable,
                        forwardedSubscriptionRequest);
                }
            }
        }
    }
}
