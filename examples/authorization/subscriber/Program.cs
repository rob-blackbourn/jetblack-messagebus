#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common.Json;

namespace AuthSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("subscriber");

            Console.WriteLine("Enter a username and password.");
            Console.WriteLine("Known users are:");
            Console.WriteLine("  username=\"tom\", password=\"tomsPassword\", roles=Subscribe");
            Console.WriteLine("  username=\"dick\", password=\"dicksPassword\", roles=Subscribe");
            Console.WriteLine("  username=\"harry\", password=\"harrysPassword\", roles=Notify|Publish");
            Console.WriteLine("  username=\"mary\", password=\"marysPassword\", roles=Authorize");
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            var server = Environment.ExpandEnvironmentVariables("%FQDN%");
            var authenticator = new BasicClientAuthenticator(username, password);
            var client = Client.Create(server, 9091, new JsonByteEncoder(), authenticator: authenticator, isSslEnabled: true);

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

            Console.WriteLine("Disconnecting");
            client.Dispose();
        }

        private static void OnDataReceived(object? sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                Console.WriteLine("Client disconnected");
                return;
            }

            foreach (var packet in e.Data)
            {
                if (packet.Data == null)
                    continue;
                var data = (IDictionary<string, object>)packet.Data;
                Console.WriteLine(
                "Data: " +
                    string.Join(",", data.Select(x => $"{x.Key}={x.Value}")));
            }
        }

        private static void OnDataError(object? sender, DataErrorEventArgs e)
        {
            Console.WriteLine("Error: " + e);
        }
    }
}
