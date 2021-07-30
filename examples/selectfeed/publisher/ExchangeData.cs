using System;
using System.Collections.Generic;
using System.Text;

namespace SelectfeedPublisher
{
    class ExchangeData
    {
        static public readonly Dictionary<string, Dictionary<string, object>> Data = new Dictionary<string, Dictionary<string, object>>
        {
            {
                "VOD",
                new Dictionary<string, object>
                {
                    {"NAME", "Vodafone Group PLC"},
                    {"BID", 140.60},
                    {"ASK", 140.65}
                }
            },
            {
                "TSCO", new Dictionary<string, object>
                {
                    {"NAME", "Tesco PLC"},
                    {"BID", 423.15},
                    {"ASK", 423.25}
                }
            },
            {
                "SBRY",
                new Dictionary<string, object>
                {
                    {"NAME", "J Sainsbury PLC"},
                    {"BID", 325.30},
                    {"ASK", 325.35}
                }
            }
        };
    }
}
