using System;
using System.Collections.Generic;
using System.Linq;

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
            client.OnDataError += OnDataError;

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

        private static void OnDataReceived(object? sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            foreach (var packet in e.Data)
            {
                if (packet.Body is Dictionary<string, object>)
                {
                    var data = (Dictionary<string, object>)packet.Body;
                    Console.WriteLine("Data: " + string.Join(",", data.Select(x => $"{x.Key}={x.Value}")));
                }
                else
                    Console.WriteLine(packet.Body);
            }
        }

        private static void OnDataError(object? sender, DataErrorEventArgs e)
        {
            Console.WriteLine("Error: " + e);
        }
    }
}
