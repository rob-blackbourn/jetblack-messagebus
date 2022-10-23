using System;
using System.Text;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common.IO;

using common;

namespace publisher
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

            Console.WriteLine("Enter the feed and topic to publish on, then the message to send.");
            Console.WriteLine("Press ENTER to quit");
            while (true)
            {
                Console.Write("Feed: ");
                var feed = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(feed)) break;

                Console.Write("Topic: ");
                var topic = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(topic)) break;

                Console.Write("Message: ");
                var message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(topic)) break;

                var data = new[]
                {
                    new DataPacket(null, message == null ? null : Encoding.UTF8.GetBytes(message))
                };

                Console.WriteLine($"Publishing on feed \"{feed}\" topic \"{topic}\" message \"{message}\"");
                client.Publish(feed, topic, true, data);
            }

            client.Dispose();
        }
    }
}
