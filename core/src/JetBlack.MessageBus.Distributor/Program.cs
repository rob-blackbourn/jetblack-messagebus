#nullable enable

using System;
using System.Net;
using log4net;
using Microsoft.Extensions.Configuration;

using JetBlack.MessageBus.Common;
using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Common.Security.Cryptography;
using JetBlack.MessageBus.Distributor.Configuration;
using System.Security.Cryptography.X509Certificates;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace JetBlack.MessageBus.Distributor
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public const string DefaultSettingsFilename = "appsettings.json";

        static void Main(string[] args)
        {
            var settingFilename = (args != null && args.Length >= 1) ? args[0] : DefaultSettingsFilename;

            var server = CreateServer(settingFilename);

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);

            server.Dispose();
        }

        private static X509Certificate2? CreateCertificate(SslConfig? config)
        {
            if (config == null || !config.IsEnabled)
                return null;

            if (config.CertFile == null || config.KeyFile == null)
                throw new ApplicationException("Invalid SSL configuration");

            var certFile = Environment.ExpandEnvironmentVariables(config.CertFile);
            var keyFile = Environment.ExpandEnvironmentVariables(config.KeyFile);

            return CertBuilder.FromFile(certFile, keyFile);
        }

        static Server CreateServer(string settingsFilename)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFilename)
                .Build();
            var distributorSection = configuration.GetSection("distributor");
            var distributorConfig = distributorSection.Get<DistributorConfig>();
            if (distributorConfig == null)
                throw new ApplicationException("No configuration");

            var endPoint = new IPEndPoint(distributorConfig.Address?.AsIPAddress() ?? IPAddress.Any, distributorConfig.Port);
            var certificate = CreateCertificate(distributorConfig.SslConfig);

            var authenticator = distributorConfig.Authentication?.Construct<IAuthenticator>() ?? new NullAuthenticator(new string[0]);
            var distributorRole = distributorConfig.ToDistributorRole();

            var server = new Server(endPoint, authenticator, certificate, distributorRole);
            server.Start(distributorConfig.HeartbeatInterval);

            return server;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal($"Unhandled error received - IsTerminating={args.IsTerminating}", args.ExceptionObject as Exception);
        }
    }
}
