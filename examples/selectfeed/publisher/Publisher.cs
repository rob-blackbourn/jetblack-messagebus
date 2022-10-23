using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common.IO;

using common;

namespace SelectfeedPublisher
{
    class Publisher : IDisposable
    {
        private const string Feed = "LSE";

        private readonly Client _client;
        private readonly Dictionary<string, Dictionary<Guid, int>> _subscriptions = new Dictionary<string, Dictionary<Guid, int>>();
        private readonly ExchangeFeed _exchangeFeed;
        private object _gate = new object();

        public Publisher(Client client, ExchangeFeed exchangeFeed)
        {
            _exchangeFeed = exchangeFeed;
            _client = client;

            _exchangeFeed.OnData += HandleExchangeData;
            _client.OnForwardedSubscription += HandleForwardedSubscription;
            _client.AddNotification(Feed);
        }

        private void HandleExchangeData(object? sender, ExchangeEventArgs args)
        {
            lock (_gate)
            {
                if (!_subscriptions.ContainsKey(args.Ticker))
                    return;

                Console.WriteLine($"Publishing {args.Ticker}");
                _client.Publish(Feed, args.Ticker, false, ToDataPackets(args.Delta));
            }
        }

        private void HandleForwardedSubscription(object? sender, ForwardedSubscriptionEventArgs args)
        {
            lock (_gate)
            {
                if (args.IsAdd)
                {
                    AddSubscription(args.ClientId, args.Feed, args.Topic);
                }
                else
                {
                    RemoveSubscription(args.ClientId, args.Feed, args.Topic);
                }
            }
        }

        public void AddSubscription(Guid clientId, string feed, string topic)
        {
            lock (_gate)
            {
                if (!_subscriptions.TryGetValue(topic, out var topicSubscriptions))
                {
                    topicSubscriptions = new Dictionary<Guid, int>();
                    _subscriptions.Add(topic, topicSubscriptions);
                }

                if (topicSubscriptions.ContainsKey(clientId))
                {
                    topicSubscriptions[clientId] += 1;
                }
                else
                {
                    Console.WriteLine($"Sending image of {topic} to {clientId}");
                    topicSubscriptions[clientId] = 1;
                    _client.Send(clientId, feed, topic, true, ToDataPackets(_exchangeFeed.Data[topic]));
                }
            }
        }

        public void RemoveSubscription(Guid clientId, string feed, string topic)
        {
            lock (_gate)
            {
                if (!_subscriptions.TryGetValue(topic, out var topicSubscriptions))
                {
                    return;
                }

                if (!topicSubscriptions.ContainsKey(clientId))
                {
                    return;
                }

                topicSubscriptions[clientId] -= 1;
                if (topicSubscriptions[clientId] > 0)
                {
                    return;
                }

                topicSubscriptions.Remove(clientId);
                if (topicSubscriptions.Count > 0)
                {
                    return;
                }

                _subscriptions.Remove(topic);

                Console.WriteLine($"Stopped publishing {topic}");
            }
        }

        private static DataPacket[] ToDataPackets(Dictionary<string, object> data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new DataPacket[]
            {
                new DataPacket(null, Encoding.UTF8.GetBytes(json))
            };
        }

        public void Dispose()
        {
            _exchangeFeed.OnData -= HandleExchangeData;
            _client.Dispose();
        }
    }
}
