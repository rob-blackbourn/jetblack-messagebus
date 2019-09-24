#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor.Subscribers
{
    internal class SubscriptionRepository
    {
        // Feed->Topic->Interactor->SubscriptionCount.
        private readonly IDictionary<string, IDictionary<string, IDictionary<Interactor, SubscriptionState>>> _cache = new Dictionary<string, IDictionary<string, IDictionary<Interactor, SubscriptionState>>>();
        private readonly GaugeDictionary _subscriptionCount = new GaugeDictionary("subscriber_count", "The number of subscribers");

        public SubscriptionRepository()
        {
        }

        public void AddSubscription(Interactor subscriber, string feed, string topic, AuthorizationInfo authorizationInfo, bool isAuthorizationUpdate)
        {
            _subscriptionCount[feed].Inc();

            // Find topic subscriptions for this feed.
            if (!_cache.TryGetValue(feed, out var topicSubscriptions))
                _cache.Add(feed, topicSubscriptions = new Dictionary<string, IDictionary<Interactor, SubscriptionState>>());

            // Find the list of interactors that have subscribed to this topic.
            if (!topicSubscriptions.TryGetValue(topic, out var subscribersForTopic))
                topicSubscriptions.Add(topic, subscribersForTopic = new Dictionary<Interactor, SubscriptionState>());

            // Find this interactor.
            if (!subscribersForTopic.TryGetValue(subscriber, out var subscriptionState))
                subscribersForTopic.Add(subscriber, subscriptionState = new SubscriptionState(authorizationInfo));

            if (isAuthorizationUpdate)
                subscriptionState.AuthorizationInfo = authorizationInfo;
            else
                subscriptionState.Count = subscriptionState.Count + 1;
        }

        public void RemoveSubscription(Interactor subscriber, string feed, string topic, bool removeAll)
        {
            // Can we find topic subscriptions this feed?
            if (!_cache.TryGetValue(feed, out var topicSubscriptions))
                return;

            // Can we find subscribers for this topic?
            if (!topicSubscriptions.TryGetValue(topic, out var subscribersForTopic))
                return;

            // Has this subscriber registered an interest in the topic?
            if (!subscribersForTopic.TryGetValue(subscriber, out var subscriptionState))
                return;

            if (removeAll)
                _subscriptionCount[feed].Dec(subscriptionState.Count);
            else
                _subscriptionCount[feed].Dec();

            if (removeAll || --subscriptionState.Count == 0)
                subscribersForTopic.Remove(subscriber);

            // If there are no subscribers left on this topic, remove it from the feed.
            if (subscribersForTopic.Count == 0)
                topicSubscriptions.Remove(topic);

            // If there are no topics left in the feed, remove it from the cache.
            if (topicSubscriptions.Count == 0)
                _cache.Remove(feed);
        }

        public IEnumerable<FeedTopic> FindByInteractor(Interactor interactor)
        {
            return _cache
                .Select(topicCache =>
                    topicCache.Value
                        .Where(x => x.Value.ContainsKey(interactor))
                        .Select(x => new FeedTopic(topicCache.Key, x.Key)))
                .SelectMany(x => x);
        }

        public IEnumerable<KeyValuePair<Interactor, AuthorizationInfo>> GetSubscribersToFeedAndTopic(string feed, string topic)
        {
            // Can we find this feed in the cache?
            if (_cache.TryGetValue(feed, out var topicCache))
            {
                // Are there subscribers for this topic?
                if (topicCache.TryGetValue(topic, out var subscribersForTopic))
                    return subscribersForTopic.Select(x => KeyValuePair.Create(x.Key, x.Value.AuthorizationInfo));
            }
            return new KeyValuePair<Interactor, AuthorizationInfo>[0];
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<Interactor>>> GetSubscribersToFeed(string feed)
        {
            // Can we find this feed in the cache?
            if (_cache.TryGetValue(feed, out var topicCache))
                return topicCache.Select(x => KeyValuePair.Create(x.Key, x.Value.Keys.AsEnumerable()));

            return new KeyValuePair<string, IEnumerable<Interactor>>[0];
        }

        private class SubscriptionState
        {
            public SubscriptionState(AuthorizationInfo authorizationInfo)
            {
                AuthorizationInfo = authorizationInfo;
            }

            public int Count { get; set; }
            public AuthorizationInfo AuthorizationInfo { get; set; }
        }
    }
}
