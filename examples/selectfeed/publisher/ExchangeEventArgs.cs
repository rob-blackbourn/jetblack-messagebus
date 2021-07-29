using System;
using System.Collections.Generic;

namespace SelectfeedPublisher
{
    class ExchangeEventArgs : EventArgs
    {
        public ExchangeEventArgs(string ticker, Dictionary<string, object> delta)
        {
            Ticker = ticker;
            Delta = delta;
        }

        public string Ticker { get;  }
        public Dictionary<string, object> Delta { get; }
    }
}
