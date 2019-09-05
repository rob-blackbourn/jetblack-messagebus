#nullable enable

using System;
using System.Collections.Generic;

using JetBlack.MessageBus.Adapters;

namespace publisher
{
    public class CachingPublisher
    {
        private readonly Client _client;
        private readonly Cache _cache;
        private readonly object _gate = new object();

        public CachingPublisher(Client client)
        {
            _client = client;
            _cache = new Cache(client, null);

            _client.OnForwardedSubscription += (sender, args) =>
            {
                lock (_gate)
                {
                    if (args.IsAdd)
                        _cache.AddSubscription(args.ClientId, args.Feed, args.Topic);
                    else
                        _cache.RemoveSubscription(args.ClientId, args.Feed, args.Topic);
                }
            };
        }

        public void Publish(string feed, string topic, Dictionary<string, object> data)
        {
            lock (_gate)
            {
                _cache.Publish(feed, topic, data);
            }
        }

        public void AddNotification(string feed)
        {
            _client.AddNotification(feed);
        }

        public void RemoveNotification(string feed)
        {
            _client.RemoveNotification(feed);
        }
    }
}
