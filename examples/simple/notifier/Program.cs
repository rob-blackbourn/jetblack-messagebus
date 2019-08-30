﻿using System;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Adapters.Configuration;
using JetBlack.MessageBus.Common.IO;

namespace notifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var authenticator = new NullClientAuthenticator();
            var client = Client.Create("localhost", 9091);
            client.OnForwardedSubscription += OnForwardedSubscription;

            Console.WriteLine("Enter the feed to be notified on.");
            Console.WriteLine("Press ENTER to quit");
            while (true)
            {
                Console.Write("Feed: ");
                var feed = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(feed)) break;
                client.AddNotification(feed);
            }

            client.Dispose();
        }

        private static void OnForwardedSubscription(object? sender, ForwardedSubscriptionEventArgs args)
        {
            Console.WriteLine(args);
        }
    }
}
