#nullable enable

using System;
using System.Security.Cryptography.X509Certificates;
using JetBlack.MessageBus.Common.Security.Cryptography;

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class SslConfig
    {
        public bool IsEnabled { get; set; }
        public string? CertFile { get; set; }
        public string? KeyFile { get; set; }
        public string? StoreLocation { get; set; }
        public string? SubjectName { get; set; }

        public X509Certificate2? ToCertificate()
        {
            if (!IsEnabled)
                return null;

            if (StoreLocation == null || string.IsNullOrWhiteSpace(StoreLocation))
            {
                if (CertFile == null || KeyFile == null)
                    throw new ApplicationException("Invalid SSL configuration");

                var certFile = Environment.ExpandEnvironmentVariables(CertFile);
                var keyFile = Environment.ExpandEnvironmentVariables(KeyFile);

                return CertBuilder.FromFile(certFile, keyFile);
            }
            else
            {
                var storeLocation = Enum.Parse<StoreLocation>(StoreLocation);
                if (SubjectName == null)
                    throw new ApplicationException("No subject name in config");

                return CertBuilder.FromStore(storeLocation, SubjectName);
            }
        }
    }
}