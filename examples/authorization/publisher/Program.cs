using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common;
using JetBlack.MessageBus.Common.IO;

using Common;

namespace AuthPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("publisher");

            try
            {
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

                var data = new[]
                {
                    new DataPacket(new HashSet<int>{Constants.Level1}, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(level1Data))),
                    new DataPacket(new HashSet<int>{Constants.Level2}, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(level2Data)))
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
