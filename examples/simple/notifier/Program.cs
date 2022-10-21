using System;
using System.Net;

using JetBlack.MessageBus.Adapters;

namespace notifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Dns.GetHostEntry("LocalHost").HostName;
            var client = Client.Create(host, 9001, isSslEnabled: true);

            client.OnForwardedSubscription += OnForwardedSubscription;

            Console.WriteLine("Enter the feed to be notified on.");
            Console.WriteLine("Press ENTER to quit");
            while (true)
            {
                Console.Write("Feed: ");
                var feed = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(feed)) break;

                Console.WriteLine($"Requesting notifications for subscriptions on feed \"{feed}\".");
                client.AddNotification(feed);
            }

            client.Dispose();
        }

        private static void OnForwardedSubscription(object? sender, ForwardedSubscriptionEventArgs args)
        {
            Console.WriteLine($"Received forwarded subscription from client {args.ClientId} on feed \"{args.Feed}\" for topic \"{args.Topic}\"");
        }
    }
}
