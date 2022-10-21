using System;
using System.Net;
using System.Text;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common.IO;

namespace publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client.Create(Dns.GetHostName(), 9001);

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
