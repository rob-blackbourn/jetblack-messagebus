#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Messages;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Distributor.Publishers
{
    public class PublisherManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PublisherManager));

        private readonly PublisherRepository _repository;

        public PublisherManager(InteractorManager interactorManager)
        {
            _repository = new PublisherRepository();
            interactorManager.ClosedInteractors += OnClosedInteractor;
            interactorManager.FaultedInteractors += OnFaultedInteractor;
        }

        public event EventHandler<StalePublisherEventArgs>? StalePublishers;

        public void SendUnicastData(Interactor publisher, Interactor subscriber, AuthorizationInfo authorization, UnicastData unicastData)
        {
            if (!publisher.HasRole(unicastData.Feed, Role.Publish))
            {
                Log.Warn($"Rejected request from {publisher} to publish on feed {unicastData.Feed}");
                return;
            }

            var clientUnicastData = new ForwardedUnicastData(
                publisher.User,
                publisher.Host,
                unicastData.ClientId,
                unicastData.Feed,
                unicastData.Topic,
                unicastData.IsImage,
                GetAuthorizedData(unicastData.Data, authorization));

            Log.Debug($"Sending unicast data from {publisher} to {subscriber}: {clientUnicastData}");

            _repository.AddPublisher(publisher, unicastData.Feed, unicastData.Topic);

            try
            {
                subscriber.SendMessage(clientUnicastData);
            }
            catch (Exception exception)
            {
                Log.Debug($"Failed to send to subscriber {subscriber} unicast data {clientUnicastData}", exception);
            }
        }

        public void SendMulticastData(Interactor? publisher, IEnumerable<KeyValuePair<Interactor, AuthorizationInfo>> subscribers, MulticastData multicastData)
        {
            if (!(publisher == null || publisher.HasRole(multicastData.Feed, Role.Publish)))
            {
                Log.Warn($"Rejected request from {publisher} to publish to Feed {multicastData.Feed}");
                return;
            }

            foreach (var subscriberAndAuthorizationInfo in subscribers)
            {
                var subscriber = subscriberAndAuthorizationInfo.Key;
                var authorization = subscriberAndAuthorizationInfo.Value;

                var subscriberMulticastData = new ForwardedMulticastData(
                    publisher?.User ?? "internal",
                    publisher?.Host ?? "localhost",
                    multicastData.Feed,
                    multicastData.Topic,
                    multicastData.IsImage,
                    GetAuthorizedData(multicastData.Data, authorization));

                Log.Debug($"Sending multicast data from {publisher} to {subscriber}: {subscriberMulticastData}");

                if (publisher != null)
                    _repository.AddPublisher(publisher, subscriberMulticastData.Feed, subscriberMulticastData.Topic);

                try
                {
                    subscriber.SendMessage(subscriberMulticastData);
                }
                catch (Exception exception)
                {
                    Log.Debug($"Failed to send to subscriber {subscriber} multicast data {subscriberMulticastData}", exception);
                }
            }
        }

        private BinaryDataPacket[]? GetAuthorizedData(BinaryDataPacket[]? data, AuthorizationInfo authorization)
        {
            return authorization.IsAuthorizationRequired
                    ? data.Where(x => authorization.Entitlements.Contains(x.Header)).ToArray()
                    : data;
        }

        private void OnClosedInteractor(object? sender, InteractorClosedEventArgs args)
        {
            CloseInteractor(args.Interactor);
        }

        private void OnFaultedInteractor(object? sender, InteractorFaultedEventArgs args)
        {
            Log.Debug($"Interactor faulted: {args.Interactor} - {args.Error.Message}");
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
