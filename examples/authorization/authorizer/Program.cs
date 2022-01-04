#nullable enable

using System;
using System.Collections.Generic;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common;

using Common;

namespace AuthEntitler
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("authorizer");

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
                client.Authorize(e.ClientId, e.Feed, e.Topic, true, new HashSet<int> { Constants.Level1, Constants.Level2 });
            }
            else if (e.User == "dick")
            {
                Console.WriteLine("dick can only see level1");
                client.Authorize(e.ClientId, e.Feed, e.Topic, true, new HashSet<int> { Constants.Level1 });
            }
            else
            {
                Console.WriteLine("others have no entitlements");
                client.Authorize(e.ClientId, e.Feed, e.Topic, true, null);
            }

            Console.WriteLine($"OnAuthorizationRequest: {e}");
        }
    }
}
