using System;
using System.Collections.Generic;
using System.Timers;

using JetBlack.MessageBus.Common.Json;
using JetBlack.MessageBus.Adapters;

namespace publisher
{
    class Program
    {
        private const int PublishMs = 100;
        private static Random Rnd = new Random();

        static void Main(string[] args)
        {
            var authenticator = new NullClientAuthenticator();
            var client = Client.Create("localhost", 9091, new JsonByteEncoder());
            var cachingPublisher = new CachingPublisher(client);
            StartPublishing(cachingPublisher);
            System.Threading.Thread.CurrentThread.Join();
        }

        private static List<Timer> StartPublishing(CachingPublisher cachingPublisher)
        {
            // Prepare some data.
            Dictionary<string, Dictionary<string, object>> lseData = new Dictionary<string, Dictionary<string, object>>
        {
            {
                "VOD",
                new Dictionary<string, object>
                {
                    { "NAME", "Vodafone Group PLC"},
                    {"BID", 140.60},
                    {"ASK", 140.65}
                }
            },
            {
                "TSCO", new Dictionary<string, object>
                {
                    {"NAME", "Tesco PLC"},
                    {"BID", 423.15},
                    {"ASK", 423.25}
                }
            },
            {
                "SBRY",
                new Dictionary<string, object>
                {
                    {"NAME", "J Sainsbury PLC"},
                    {"BID", 325.30},
                    {"ASK", 325.35}
                }
            }
        };

            Dictionary<string, Dictionary<string, Dictionary<string, object>>> marketData = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            marketData.Add("LSE", lseData);

            // Publish the data.
            foreach (var feedItem in marketData)
            {
                var feed = feedItem.Key;
                var topicData = feedItem.Value;

                cachingPublisher.AddNotification(feed);

                foreach (var topicItem in topicData)
                {
                    var topic = topicItem.Key;
                    var data = topicItem.Value;

                    cachingPublisher.Publish(feed, topic, data);
                }
            }

            var timers = new List<Timer>();
            foreach (var feedItem in marketData)
            {
                var feed = feedItem.Key;
                var topicData = feedItem.Value;

                foreach (var topicItem in topicData)
                {
                    var topic = topicItem.Key;
                    var data = topicItem.Value;

                    var timer = new Timer();
                    timers.Add(timer);

                    ScheduleUpdate(cachingPublisher, feed, topic, data, timer);
                }
            }

            return timers;
        }

        private static void ScheduleUpdate(CachingPublisher cachingPublisher, string feed, string topic, Dictionary<string, object> data, Timer timer)
        {
            timer.Elapsed += (sender, args) =>
            {
                PublishUpdate(cachingPublisher, feed, topic, data);
                ScheduleUpdate(cachingPublisher, feed, topic, data, timer);
            };
            timer.Interval = (long)(PublishMs * Rnd.NextDouble() * 100 + 5);
            timer.Enabled = true;
        }

        private static void PublishUpdate(CachingPublisher cachingPublisher, string feed, string topic, Dictionary<string, object> data)
        {
            // Perturb the data a little.
            var bid = (double)data["BID"];
            var ask = (double)data["ASK"];
            var spread = ask - bid;
            var nextBid = Math.Round((bid + bid * Rnd.NextDouble() * 5.0 / 100.0) * 100.0) / 100.0;
            double nextAsk = Math.Round((nextBid + spread) * 100.0) / 100.0;
            var newData = new Dictionary<string, object>
            {
                {"BID", nextBid},
                {"ASK", nextAsk}
            };
            cachingPublisher.Publish(feed, topic, newData);
        }
    }
}
