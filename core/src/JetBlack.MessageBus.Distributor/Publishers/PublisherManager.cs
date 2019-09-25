#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Messages;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Distributor.Publishers
{
    public class PublisherManager
    {
        private readonly ILogger<PublisherManager> _logger;
        private readonly PublisherRepository _repository;

        public PublisherManager(
            InteractorManager interactorManager,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PublisherManager>();
            _repository = new PublisherRepository();
            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
        }

        public event EventHandler<StalePublisherEventArgs>? StalePublishers;

        public void SendUnicastData(Interactor publisher, Interactor subscriber, AuthorizationInfo authorization, UnicastData unicastData)
        {
            if (!publisher.HasRole(unicastData.Feed, Role.Publish))
            {
                _logger.LogWarning("Rejected request from {Publisher} to publish on feed {Feed}", publisher, unicastData.Feed);
                return;
            }

            publisher.Metrics.UnicastMessages[unicastData.Feed].Inc();

            var clientUnicastData = new ForwardedUnicastData(
                publisher.UserForFeed(unicastData.Feed),
                publisher.HostForFeed(unicastData.Feed),
                unicastData.ClientId,
                unicastData.Feed,
                unicastData.Topic,
                unicastData.IsImage,
                GetAuthorizedData(unicastData.DataPackets, authorization));

            _logger.LogDebug("Sending unicast data from {Publisher} to {Subscriber}: {Message}", publisher, subscriber, clientUnicastData);

            _repository.AddPublisher(publisher, unicastData.Feed, unicastData.Topic);

            try
            {
                subscriber.SendMessage(clientUnicastData);
            }
            catch (Exception error)
            {
                _logger.LogDebug(error, "Failed to send to subscriber {Subscriber} unicast data {Message}", subscriber, clientUnicastData);
            }
        }

        public void SendMulticastData(Interactor? publisher, IEnumerable<KeyValuePair<Interactor, AuthorizationInfo>> subscribers, MulticastData multicastData)
        {
            if (!(publisher == null || publisher.HasRole(multicastData.Feed, Role.Publish)))
            {
                _logger.LogWarning("Rejected request from {Publisher} to publish to Feed {Feed}", publisher, multicastData.Feed);
                return;
            }

            if (publisher != null)
                publisher.Metrics.MulticastMessages[multicastData.Feed].Inc();

            foreach (var subscriberAndAuthorizationInfo in subscribers)
            {
                var subscriber = subscriberAndAuthorizationInfo.Key;
                var authorization = subscriberAndAuthorizationInfo.Value;

                var subscriberMulticastData = new ForwardedMulticastData(
                    publisher?.UserForFeed(multicastData.Feed) ?? "internal",
                    publisher?.HostForFeed(multicastData.Feed) ?? "localhost",
                    multicastData.Feed,
                    multicastData.Topic,
                    multicastData.IsImage,
                    GetAuthorizedData(multicastData.DataPackets, authorization));

                _logger.LogDebug("Sending multicast data from {Publisher} to {Subscriber}: {Message}", publisher, subscriber, subscriberMulticastData);

                if (publisher != null)
                    _repository.AddPublisher(publisher, subscriberMulticastData.Feed, subscriberMulticastData.Topic);

                try
                {
                    subscriber.SendMessage(subscriberMulticastData);
                }
                catch (Exception error)
                {
                    _logger.LogDebug(error, "Failed to send to subscriber {Subscriber} multicast data {Message}", subscriber, subscriberMulticastData);
                }
            }
        }

        private DataPacket[]? GetAuthorizedData(DataPacket[]? dataPackets, AuthorizationInfo authorization)
        {
            return authorization.IsAuthorizationRequired
                    ? dataPackets.Where(packet => packet.IsAuthorized(authorization.Entitlements)).ToArray()
                    : dataPackets;
        }

        private void OnClosedInteractor(object? sender, InteractorClosedEventArgs args)
        {
            CloseInteractor(args.Interactor);
        }

        private void OnFaultedInteractor(object? sender, InteractorFaultedEventArgs args)
        {
            _logger.LogDebug("Interactor faulted: {Interactor} - {Message}", args.Interactor, args.Error.Message);
            CloseInteractor(args.Interactor);
        }

        private void CloseInteractor(Interactor? interactor)
        {
            if (interactor == null) return;

            var topicsWithoutPublishers = _repository.RemovePublisher(interactor).ToList();
            if (topicsWithoutPublishers.Count > 0)
                StalePublishers?.Invoke(this, new StalePublisherEventArgs(interactor, topicsWithoutPublishers));
        }
    }
}
