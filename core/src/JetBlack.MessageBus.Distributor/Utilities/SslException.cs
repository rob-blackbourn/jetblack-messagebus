#nullable enable

using System;
using System.Net;

namespace JetBlack.MessageBus.Distributor.Utilities
{
    public class SslException : Exception
    {
        public SslException(IPAddress address)
            : base()
        {
            Address = address;
        }

        public IPAddress Address { get; }
    }
}