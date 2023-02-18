using System;
using System.Threading;
using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

using Prometheus;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Configuration;
using JetBlack.MessageBus.Distributor.Utilities;

namespace JetBlack.MessageBus.Distributor
{
    public class ServerManager : IDisposable
    {
        public ILogger<ServerManager> Logger { get; }
        private readonly DistributorConfig _distributorConfig;
        private readonly Server _server;
        private MetricServer? _metricServer;

        public ServerManager(string settingsFilename)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFilename, false, true)
                .Build();

            _distributorConfig = configuration.GetSection("distributor").Get<DistributorConfig>();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(configuration.GetSection("Logging"))
                    .AddDistributorLogger(_distributorConfig);
            });

            Logger = loggerFactory.CreateLogger<ServerManager>();

            _server = CreateServer(_distributorConfig, loggerFactory);
        }

        public void Start()
        {
            _server.Start(_distributorConfig.HeartbeatInterval);
        }

        private Server CreateServer(DistributorConfig distributorConfig, ILoggerFactory loggerFactory)
        {
            if (distributorConfig == null)
                throw new ApplicationException("No configuration");

            if (distributorConfig.Prometheus?.IsEnabled != true)
            {
                _metricServer = null;
            }
            else
            {
                _metricServer = new MetricServer(distributorConfig.Prometheus.Port);
                _metricServer.Start();
            }

            var endPoint = distributorConfig.ToIPEndPoint();
            var certificate = distributorConfig.Ssl?.ToCertificate();
            var authenticator = distributorConfig.Authentication?.Construct<IAuthenticator>() ?? new NullAuthenticator(new string[0]);
            var distributorRole = distributorConfig.ToDistributorRole();
            var sspiEndPoint = distributorConfig.Sspi?.IsEnabled == true
                ? distributorConfig.Sspi.ToIPEndPoint()
                : null;

            var server = new Server(
                endPoint,
                authenticator,
                certificate,
                distributorRole,
                sspiEndPoint,
                loggerFactory);

            server.Start(distributorConfig.HeartbeatInterval);

            return server;
        }

        public void Dispose()
        {
            _server.Dispose();
            _metricServer?.Stop();
        }
    }
}
