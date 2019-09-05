﻿using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using JetBlack.MessageBus.Adapters;

namespace subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var authenticator = new NullClientAuthenticator();
            var client = Client.Create("localhost", 9091);

            client.OnDataReceived += OnDataReceived;

            Console.WriteLine("Enter an empty feed or topic to quit");
            while (true)
            {
                Console.Write("Feed: ");
                var feed = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(feed)) break;

                Console.Write("Topic: ");
                var topic = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(topic)) break;

                Console.WriteLine($"Subscribing to feed \"{feed}\" with topic \"{topic}\"");
                client.AddSubscription(feed, topic);
            }

            client.Dispose();
        }

        private static void OnDataReceived(object? sender, DataReceivedEventArgs args)
        {
            Console.Write($"Received data on feed \"{args.Feed}\" for topic \"{args.Topic}\"");

            if (args.DataPackets == null)
            {
                Console.Write("No Data");
                return;
            }

            foreach (var packet in args.DataPackets)
            {
                if (packet.Data != null)
                {
                    var json = Encoding.UTF8.GetString(packet.Data);
                    var message = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    Console.WriteLine($"Received: \"{message}\"");
                }
            }
        }
    }
}
