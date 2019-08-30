#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Adapters.Configuration;

using Common;

namespace AuthEntitler
{
    class Program
    {
        public const string DefaultSettingsFilename = "appsettings.json";

        static void Main(string[] args)
        {
            Console.WriteLine("authorizer");

            var settingsFilename = (args != null && args.Length >= 1) ? args[0] : DefaultSettingsFilename;

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFilename)
                .Build();
            var section = configuration.GetSection("client");
            var config = section.Get<ClientConfig>();

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

            client.OnAuthorizationRequest += OnAuthorizationRequest;

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();
            client.Dispose();

            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
        }

        private static void OnAuthorizationRequest(object? sender, AuthorizationRequestEventArgs e)
        {
            if (!(sender is Client)) return;
            var client = (Client)sender;

            if (e.User == "tom")
            {
                Console.WriteLine("tom can see both level1 and level2");
                client.Authorize(e.ClientId, e.Feed, e.Topic, true, new[] { Constants.Level1, Constants.Level2 });
            }
            else if (e.User == "dick")
            {
                Console.WriteLine("dick can only see level1");
                client.Authorize(e.ClientId, e.Feed, e.Topic, true, new[] { Constants.Level1 });
            }
            else
            {
                Console.WriteLine("others have no entitlements");
                client.Authorize(e.ClientId, e.Feed, e.Topic, true, new Guid[0]);
            }

            Console.WriteLine($"OnAuthorizationRequest: {e}");
        }
    }
}
