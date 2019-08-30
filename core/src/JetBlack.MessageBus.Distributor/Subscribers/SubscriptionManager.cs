#nullable enable

using System.Linq;

using log4net;

using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Distributor.Notifiers;
using JetBlack.MessageBus.Distributor.Publishers;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Messages;
using System;
using System.Collections.Generic;

namespace JetBlack.MessageBus.Distributor.Subscribers
{
    internal class SubscriptionManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SubscriptionManager));

        private readonly SubscriptionRepository _repository;
        private readonly InteractorManager _interactorManager;
        private readonly NotificationManager _notificationManager;
        private readonly PublisherManager _publisherManager;

        public SubscriptionManager(InteractorManager interactorManager, NotificationManager notificationManager)
        {
            _repository = new SubscriptionRepository();

            _interactorManager = interactorManager;
            _notificationManager = notificationManager;
            _publisherManager = new PublisherManager(interactorManager);

            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
            interactorManager.AuthorizationResponses += OnAuthorizationResponse;

            notificationManager.NewNotificationRequests += OnNewNotificationRequest;

            _publisherManager.StalePublishers += OnStalePublishers;
        }

        public void RequestSubscription(Interactor subscriber, SubscriptionRequest subscriptionRequest)
        {
            if (!subscriber.HasRole(subscriptionRequest.Feed, Role.Subscribe))
            {
                Log.Warn($"Rejected request from {subscriber} to subscribe to feed \"{subscriptionRequest.Feed}\"");
                return;
            }

            Log.Info($"Received subscription from {subscriber} on \"{subscriptionRequest}\"");

            if (subscriptionRequest.IsAdd)
                _interactorManager.RequestAuthorization(subscriber, subscriptionRequest.Feed, subscriptionRequest.Topic);
            else
            {
                _repository.RemoveSubscription(subscriber, subscriptionRequest.Feed, subscriptionRequest.Topic, false);
                _notificationManager.ForwardSubscription(subscriber, subscriptionRequest);
            }
        }

        private void OnFaultedInteractor(object? sender, InteractorFaultedEventArgs args)
        {
            Log.Debug($"Interactor faulted: {args.Interactor} - {args.Error.Message}");

            CloseInteractor(args.Interactor);
        }

        private void OnClosedInteractor(object? sender, InteractorClosedEventArgs args)
        {
            CloseInteractor(args.Interactor);
        }

        private void CloseInteractor(Interactor interactor)
        {
            Log.Debug($"Removing subscriptions for {interactor}");

            // Remove the subscriptions
            var feedTopics = _repository.FindByInteractor(interactor).ToList();
            foreach (var feedTopic in feedTopics)
                _repository.RemoveSubscription(interactor, feedTopic.Feed, feedTopic.Topic, true);

            // Inform those interested that this interactor is no longer subscribed to these topics.
            foreach (var subscriptionRequest in feedTopics.Select(feedTopic => new SubscriptionRequest(feedTopic.Feed, feedTopic.Topic, false)))
                _notificationManager.ForwardSubscription(interactor, subscriptionRequest);
        }

        public void OnAuthorizationResponse(object? sender, AuthorizationResponseEventArg args)
        {
            if (args.Response.IsAuthorizationRequired && (args.Response.Entitlements == null || args.Response.Entitlements.Length == 0))
            {
                var message = new ForwardedMulticastData(string.Empty, string.Empty, args.Response.Feed, args.Response.Topic, true, null);
                try
                {
                    args.Requester.SendMessage(message);
                }
                catch (Exception error)
                {
                    Log.Debug($"Failed to send to {args.Requester} multi cast message {message}", error);
                }

                return;
            }

            _repository.AddSubscription(
                args.Requester,
                args.Response.Feed,
                args.Response.Topic,
                new AuthorizationInfo(
                    args.Response.IsAuthorizationRequired,
                    new HashSet<Guid>(args.Response.Entitlements ?? new Guid[0])));

            _notificationManager.ForwardSubscription(
                args.Requester,
                new SubscriptionRequest(args.Response.Feed, args.Response.Topic, true));
        }

        public void SendUnicastData(Interactor publisher, UnicastData unicastData)
        {
            // Can we find this client in the subscribers to this topic?
            var subscriber = _repository.GetSubscribersToFeedAndTopic(unicastData.Feed, unicastData.Topic)
                .FirstOrDefault(x => x.Key.Id == unicastData.ClientId);
            if (subscriber.Key == null)
                return;

            _publisherManager.SendUnicastData(publisher, subscriber.Key, subscriber.Value, unicastData);
        }

        public void SendMulticastData(Interactor publisher, MulticastData multicastData)
        {
            _publisherManager.SendMulticastData(
                publisher,
                _repository.GetSubscribersToFeedAndTopic(multicastData.Feed, multicastData.Topic),
                multicastData);
        }

        public void OnNewNotificationRequest(object? sender, NotificationEventArgs args)
        {
            // Find the subscribers who's subscriptions match the pattern.
            foreach (var matchingSubscriptions in _repository.GetSubscribersToFeed(args.Feed))
            {
                // Tell the requestor about subscribers that are interested in this topic.
                foreach (var subscriber in matchingSubscriptions.Value)
                {
                    var message = new ForwardedSubscriptionRequest(subscriber.User, subscriber.Host, subscriber.Id, args.Feed, matchingSubscriptions.Key, true);
                    try
                    {
                        args.Interactor.SendMessage(message);
                    }
                    catch (Exception exception)
                    {
                        Log.Debug($"Failed to inform {subscriber} regarding {message}", exception);
                    }
                }
            }
        }

        public void OnStalePublishers(object? sender, StalePublisherEventArgs args)
        {
            foreach (var staleFeedTopic in args.FeedsAndTopics)
                OnStaleTopic(staleFeedTopic);
        }

        private void OnStaleTopic(FeedTopic staleFeedTopic)
        {
            // Inform subscribers by sending an image with no data.
            var staleMessage = new ForwardedMulticastData(string.Empty, string.Empty, staleFeedTopic.Feed, staleFeedTopic.Topic, true, null);

            foreach (var subscriber in _repository.GetSubscribersToFeedAndTopic(staleFeedTopic.Feed, staleFeedTopic.Topic).Select(x => x.Key))
            {
                try
                {
                    subscriber.SendMessage(staleMessage);
                }
                catch (Exception exception)
                {
                    Log.Debug($"Failed to inform {subscriber} of stale {staleFeedTopic}", exception);
                }
            }
        }
    }
}
