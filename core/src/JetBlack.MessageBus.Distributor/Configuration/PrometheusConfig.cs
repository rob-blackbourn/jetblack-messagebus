namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class PrometheusConfig
    {
        public int Port { get; set; }
        public bool IsEnabled { get; set; } = false;

        public override string ToString() => $"{nameof(Port)}={Port}";
    }
}