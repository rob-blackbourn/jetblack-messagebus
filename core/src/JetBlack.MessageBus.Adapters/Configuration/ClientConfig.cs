#nullable enable

namespace JetBlack.MessageBus.Adapters.Configuration
{
    public class ClientConfig
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? ByteEncoderType { get; set; }
        public bool MonitorHeartbeat { get; set; }
        public bool IsSslEnabled { get; set; }

        public override string ToString() => $"{nameof(Server)}={Server},{nameof(Port)}={Port},{nameof(ByteEncoderType)}={ByteEncoderType},{nameof(MonitorHeartbeat)}={MonitorHeartbeat}, {nameof(IsSslEnabled)}={IsSslEnabled}";
    }
}
