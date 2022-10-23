using System.Net;

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class SspiConfig
    {
        public bool IsEnabled { get; set; }
        public string? Address { get; set; }
        public int Port { get; set; }

        public IPEndPoint ToIPEndPoint()
        {
            if (Address == null || !IPAddress.TryParse(Address, out var address))
                address = IPAddress.Any;
            return new IPEndPoint(address, Port);
        }
    }
}