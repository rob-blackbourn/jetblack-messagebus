using System;

using JetBlack.MessageBus.Adapters;

using common;

namespace SelectfeedPublisher
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

            Console.WriteLine("Starting selectfeed publisher");
            var exchangeFeed = new ExchangeFeed();
            var publisher = new Publisher(client, exchangeFeed);

            exchangeFeed.Start();

            Console.WriteLine("Press <ENTER> to quit");
            Console.ReadLine();

            exchangeFeed.Dispose();
        }
    }
}
