﻿using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

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
            Console.WriteLine($"Received on feed \"{args.Feed}\" for topic \"{args.Topic}\"");

            if (args.DataPackets == null)
            {
                Console.WriteLine("No Data");
                return;
            }

            foreach (var packet in args.DataPackets)
            {
                if (packet.Data != null)
                {
                    var json = Encoding.UTF8.GetString(packet.Data);
                    var dataFrame = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
                    if (dataFrame == null)
                    {
                        return;
                    }

                    foreach (var dataItem in dataFrame)
                    {
                        foreach (var item in dataItem.Value)
                        {
                            Console.WriteLine($"{dataItem.Key}: {item.Key} = {item.Value}");
                        }
                    }
                }
            }
        }
    }
}
