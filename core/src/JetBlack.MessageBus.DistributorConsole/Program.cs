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
using JetBlack.MessageBus.Distributor;

namespace JetBlack.MessageBus.DistributorConsole
{
    class Program
    {

        public const string DefaultSettingsFilename = "appsettings.json";

        static void Main(string[] args)
        {
            var settingsFilename = (args != null && args.Length >= 1) ? args[0] : DefaultSettingsFilename;

            var serverManager = new ServerManager(settingsFilename);

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
            serverManager.Logger.LogInformation("Waiting for SIGTERM/SIGINT on PID {ProcessId}", process.Id);
            exitEvent.WaitOne();

            serverManager.Dispose();
        }
    }
}
