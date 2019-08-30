#nullable enable

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class SslConfig
    {
        public bool IsEnabled { get; set; }
        public string? CertFile { get; set; }
        public string? KeyFile { get; set; }
    }
}