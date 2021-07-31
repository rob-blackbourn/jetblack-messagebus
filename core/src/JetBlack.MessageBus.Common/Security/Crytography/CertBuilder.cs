#nullable enable

using System;
using System.Security.Cryptography.X509Certificates;

namespace JetBlack.MessageBus.Common.Security.Cryptography
{
    public class CertBuilder
    {
        public static X509Certificate2 FromFile(string certFileName)
        {
            return CertUtils.FromFile(certFileName);
        }

#if NETSTANDARD2_1
        public static X509Certificate2 FromFile(string certFileName, string keyFile)
        {
            var cert = CertUtils.FromFile(certFileName);
            var key = KeyUtils.FromFile(keyFile);
            return cert.CopyWithPrivateKey(key);
        }
#endif

        public static X509Certificate2 FromStore(StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeLocation);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                var currentCerts = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                var signingCert = currentCerts.Find(X509FindType.FindBySubjectName, subjectName, false);
                if (signingCert.Count == 0)
                    throw new ApplicationException("No certificate");
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }

        }
    }
}