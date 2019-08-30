#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Adapters.Configuration;
using JetBlack.MessageBus.Common.IO;

using Common;

namespace AuthPublisher
{
    class Program
    {
        public const string DefaultSettingsFilename = "appsettings.json";

        static void Main(string[] args)
        {
            Console.WriteLine("publisher");

            var settingsFilename = (args != null && args.Length >= 1) ? args[0] : DefaultSettingsFilename;

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFilename)
                .Build();
            var section = configuration.GetSection("client");
            var config = section.Get<ClientConfig>();

            try
            {
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

                var authenticator = new BasicClientAuthenticator(username, password);
                var client = Client.Create(config, authenticator, true);

                client.OnConnectionChanged += OnConnectionChanged;

                // client.Start();

                var level1Data = new Dictionary<string, object>
                {
                    {"BID", 1.0},
                    {"ASK", 2.0},
                };

                var level2Data = new Dictionary<string, object>
                {
                    {"BID_PRICE1", 1.0},
                    {"BID_SIZE1", 10},
                    {"ASK_PRICE1", 2.0},
                    {"ASK_Size1", 15},
                };

                var data = new DataPacket[]
                {
                    new DataPacket(Constants.Level1, level1Data),
                    new DataPacket(Constants.Level2, level2Data)
                };

                Console.WriteLine("Enter the feed and topic to publish on");
                Console.WriteLine("Press ENTER to quit");
                while (true)
                {
                    Console.Write("Feed: ");
                    var feed = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(feed)) break;

                    Console.Write("Topic: ");
                    var topic = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(topic)) break;

                    client.Publish(feed, topic, true, data);
                }

                client.Dispose();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void OnConnectionChanged(object? sender, ConnectionChangedEventArgs e)
        {
            Console.WriteLine($"OnConnectionChanged: {e.State}");
        }

    }

}
