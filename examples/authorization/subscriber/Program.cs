#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common;

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
            var username = Console.ReadLine() ?? String.Empty;
            Console.Write("Password: ");
            var password = ConsoleHelper.ReadPassword();

            var server = Environment.ExpandEnvironmentVariables("%FQDN%");
            var authenticator = new BasicClientAuthenticator(username, password);
            var client = Client.Create(server, 9002, authenticator: authenticator, isSslEnabled: true);

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

            Console.WriteLine("Disconnecting");
            client.Dispose();
        }

        private static void OnDataReceived(object? sender, DataReceivedEventArgs args)
        {
            Console.WriteLine($"Received data on feed \"{args.Feed}\" for topic \"{args.Topic}\"");

            if (args.DataPackets == null)
            {
                Console.WriteLine("No data");
                return;
            }

            foreach (var packet in args.DataPackets)
            {
                if (packet.Data != null)
                {
                    var json = Encoding.UTF8.GetString(packet.Data);
                    var message = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    if (message == null)
                        Console.WriteLine("Empty message");
                    else
                        Console.WriteLine(
                            "Data: " +
                            string.Join(",", message.Select(x => $"{x.Key}={x.Value}")));
                }
            }
        }
    }
}
