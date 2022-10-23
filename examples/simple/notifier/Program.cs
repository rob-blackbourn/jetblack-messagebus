using System;

using JetBlack.MessageBus.Adapters;

using common;

namespace notifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var programArgs = ProgramArgs.Parse(args);

            var client = programArgs.Method != ConnectionMethod.Sspi
                ? Client.Create(
                    programArgs.Host,
                    programArgs.Port,
                    isSslEnabled: programArgs.Method == ConnectionMethod.Ssl)
                : Client.SspiCreate(programArgs.Host, programArgs.Port);

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
