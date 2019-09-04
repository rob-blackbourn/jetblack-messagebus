#nullable enable

using System;
using System.Threading;
using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Configuration;

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
            Console.WriteLine($"Waiting for SIGTERM/SIGINT on PID {process.Id}");
            exitEvent.WaitOne();

            program.Dispose();
        }


        private readonly ILogger<Program> _logger;
        private readonly DistributorConfig _distributorConfig;
        private readonly Server _server;

        public Program(string settingsFilename)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFilename)
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(configuration.GetSection("Logging"))
                    .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Program>();

            _distributorConfig = configuration.GetSection("distributor").Get<DistributorConfig>();

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
        }
    }
}
