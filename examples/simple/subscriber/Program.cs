using System;
using System.Text;

using JetBlack.MessageBus.Adapters;

using common;

namespace subscriber
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

            client.Dispose();
        }

        private static void OnDataReceived(object? sender, DataReceivedEventArgs args)
        {
            Console.Write($"Received data on feed \"{args.Feed}\" for topic \"{args.Topic}\"");

            if (args.DataPackets == null)
            {
                Console.Write("No Data");
                return;
            }

            foreach (var packet in args.DataPackets)
            {
                if (packet.Data != null)
                {
                    var message = Encoding.UTF8.GetString(packet.Data);
                    Console.WriteLine($"Received: \"{message}\"");
                }
            }
        }
    }
}
