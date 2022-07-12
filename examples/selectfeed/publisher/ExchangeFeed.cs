using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SelectfeedPublisher
{
    class ExchangeFeed : IDisposable
    {
        public event EventHandler<ExchangeEventArgs>? OnData;

        public Dictionary<string, Dictionary<string, object>> Data
        {
            get
            {
                lock(this)
                {
                    return ExchangeData.Data;
                }
            }
        }

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly Random _random = new Random();
        private readonly double MinSpread = 1;

        public void Start()
        {
            Task.Run(DataLoop);
        }

        private void DataLoop()
        {
            var tickers = Data.Keys.ToArray();

            while (!_tokenSource.IsCancellationRequested)
            {
                var randomMillis = _random.Next(200);
                Thread.Sleep(randomMillis);
                var ticker = tickers[_random.Next(tickers.Length)];
                var tickerData = Data[ticker];
                var bid = (double)tickerData["BID"];
                var ask = (double)tickerData["ASK"];

                var delta = new Dictionary<string, object>();

                var changes = _random.Next(10);
                if (changes % 2 == 0)
                {
                    bid += SmallChange();
                    delta["BID"] = bid;
                }
                if (changes % 2 != 0 || changes >= 5)
                {
                    ask += SmallChange();
                    delta["ASK"] = ask;
                }
                if (ask - bid < MinSpread)
                {
                    delta["ASK"] = bid + 1;
                }

                RaiseOnData(ticker, delta);
            }
        }

        private double SmallChange()
        {
            return Math.Round(_random.NextDouble() * 100, 2) * (_random.Next(2) - 1);
        }

        private void RaiseOnData(string ticker, Dictionary<string, object> data)
        {
            OnData?.Invoke(this, new ExchangeEventArgs(ticker, data));
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
        }
    }
}
