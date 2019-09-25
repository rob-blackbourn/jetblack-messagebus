#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common.IO;
using Newtonsoft.Json;

namespace publisher
{
    public class Cache
    {
        private readonly Dictionary<string, Dictionary<string, CacheItem>> _cacheItems = new Dictionary<string, Dictionary<string, CacheItem>>();

        private readonly Client _client;
        private readonly HashSet<int>? _entitlement;

        public Cache(Client client, HashSet<int>? entitlement)
        {
            _client = client;
            _entitlement = entitlement;
        }

        public void AddSubscription(Guid clientId, string feed, string topic)
        {
            Console.WriteLine($"AddSubscription: clientId={clientId}, topic=\"{topic}\"");

            // Have we received a subscription or published data on this feed yet?
            if (!_cacheItems.TryGetValue(feed, out var topicCache))
            {
                topicCache = new Dictionary<string, CacheItem>();
                _cacheItems.Add(feed, topicCache);
            }


            // Have we received a subscription or published data on this topic yet?
            if (!topicCache.TryGetValue(topic, out var cacheItem))
            {
                cacheItem = new CacheItem();
                topicCache.Add(topic, cacheItem);
            }

            // Has this client already subscribed to this topic?
            if (!cacheItem.ClientStates.ContainsKey(clientId))
            {
                // Add the client to the cache item, and indicate that we have not yet sent an image.
                cacheItem.ClientStates.Add(clientId, false);
            }

            if (!(cacheItem.ClientStates[clientId] || cacheItem.Data == null))
            {
                // Send the image and mark this client appropriately.
                cacheItem.ClientStates[clientId] = true;

                var json = JsonConvert.SerializeObject(cacheItem.Data);
                var data = Encoding.UTF8.GetBytes(json);

                Console.WriteLine($"Sending image on feed \"{feed}\" with topic \"{topic}\" to client {clientId}");
                _client.Send(
                    clientId,
                    feed,
                    topic,
                    true,
                    new[] { new DataPacket(_entitlement, data) });
            }
        }

        public void RemoveSubscription(Guid clientId, string feed, string topic)
        {
            Console.WriteLine($"RemoveSubscription: clientId={clientId}, topic=\"{topic}\"");

            // Have we received a subscription or published data on this feed yet?
            if (!_cacheItems.TryGetValue(feed, out var topicCache))
                return;

            // Have we received a subscription or published data on this topic yet?
            if (!topicCache.TryGetValue(topic, out var cacheItem))
                return;

            // Does this topic have this client?
            if (!cacheItem.ClientStates.ContainsKey(clientId))
                return;

            cacheItem.ClientStates.Remove(clientId);

            // If there are no clients and no data remove the item.
            if (cacheItem.ClientStates.Count == 0 || cacheItem.Data == null)
            {
                Console.WriteLine($"Stop publishing feed \"{feed}\" topic \"{topic}\"");
                _cacheItems.Remove(feed);
            }
        }

        public void Publish(string feed, string topic, Dictionary<string, object> delta)
        {
            // If the feed is not in the cache add it.
            if (!_cacheItems.TryGetValue(feed, out var topicCache))
            {
                topicCache = new Dictionary<string, CacheItem>();
                _cacheItems.Add(feed, topicCache);
            }

            // If the topic is not in the cache add it.
            if (!topicCache.TryGetValue(topic, out var cacheItem))
            {
                cacheItem = new CacheItem();
                cacheItem.Data = delta.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
                topicCache.Add(topic, cacheItem);
            }

            // Bring the cache data up to date.
            foreach (var fieldValue in delta)
            {
#nullable disable
                cacheItem.Data[fieldValue.Key] = fieldValue.Value;
#nullable enable
            }

            // If there are any clients listening publish the data.
            if (cacheItem.ClientStates.Count > 0)
            {
                var json = JsonConvert.SerializeObject(delta);
                var data = Encoding.UTF8.GetBytes(json);

                Console.WriteLine($"Publishing update on feed \"{feed}\" with topic \"{topic}\"");
                _client.Publish(
                    feed,
                    topic,
                    false,
                    new[] { new DataPacket(_entitlement, data) });
            }
        }
    }
}
