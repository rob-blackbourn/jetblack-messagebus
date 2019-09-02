#nullable enable

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
    }
}