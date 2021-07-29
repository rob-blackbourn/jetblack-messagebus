using System;

namespace SelectfeedPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting selectfeed publisher");
            var exchangeFeed = new ExchangeFeed();
            var publisher = new Publisher(exchangeFeed);

            exchangeFeed.Start();

            Console.WriteLine("Press <ENTER> to quit");
            Console.ReadLine();

            exchangeFeed.Dispose();
        }
    }
}
