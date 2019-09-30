#nullable enable

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
    class Program : IDisposable
    {

        public const string DefaultSettingsFilename = "appsettings.json";

        static void Main(string[] args)
        {
            var settingsFilename = (args != null && args.Length >= 1) ? args[0] : DefaultSettingsFilename;

            var program = new Program(settingsFilename);

            var exitEvent = new ManualResetEvent(false);
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                Console.WriteLine("Shutting down ...");
                exitEvent.Set();
            };
            Console.CancelKeyPress += (sender, args) =>
            {
                // This will call AppDomain.ProcessExit.
                Environment.Exit(0);
            };

            var process = Process.GetCurrentProcess();
            program.Logger.LogInformation("Waiting for SIGTERM/SIGINT on PID {ProcessId}", process.Id);
            exitEvent.WaitOne();

            program.Dispose();
        }


        public ILogger<Program> Logger { get; }
        private readonly DistributorConfig _distributorConfig;
        private readonly Server _server;
        private MetricServer? _metricServer;

        public Program(string settingsFilename)
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

            Logger = loggerFactory.CreateLogger<Program>();

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
            var certificate = distributorConfig.SslConfig?.ToCertificate();
            var authenticator = distributorConfig.Authentication?.Construct<IAuthenticator>() ?? new NullAuthenticator(new string[0]);
            var distributorRole = distributorConfig.ToDistributorRole();

            var server = new Server(endPoint, authenticator, certificate, distributorRole, loggerFactory);
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
