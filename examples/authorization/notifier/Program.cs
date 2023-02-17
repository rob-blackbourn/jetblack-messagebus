using System;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common;

namespace AuthNotifier
{
    class Program
    {
        public const string DefaultSettingsFilename = "appsettings.json";

        static void Main(string[] args)
        {
            Console.WriteLine("notifier");

            var settingsFilename = (args != null && args.Length >= 1) ? args[0] : DefaultSettingsFilename;

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
            var client = Client.Create(server, 9001, authenticator: authenticator, isSslEnabled: true);

            client.OnForwardedSubscription += OnForwardedSubscription;

            Console.WriteLine("Enter the feed to be notified on.");
            Console.WriteLine("The feeds \"UNAUTH\" and \"AUTH\" have been configured.");
            Console.WriteLine("Press ENTER to quit");
            while (true)
            {
                Console.Write("Feed: ");
                var feed = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(feed)) break;
                client.AddNotification(feed);
            }

            client.Dispose();

            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
        }

        private static void OnForwardedSubscription(object? sender, ForwardedSubscriptionEventArgs e)
        {
            Console.WriteLine("OnForwardedSubscription: " + e);
        }
    }
}
