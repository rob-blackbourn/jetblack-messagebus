using Microsoft.Extensions.Logging;

using JetBlack.MessageBus.Distributor.Configuration;

namespace JetBlack.MessageBus.Distributor.Utilities
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddDistributorLogger(this ILoggingBuilder builder, DistributorConfig config)
        {
            return config.UseJsonLogger ? builder.AddJsonConsole() : builder.AddConsole();
        }
    }
}