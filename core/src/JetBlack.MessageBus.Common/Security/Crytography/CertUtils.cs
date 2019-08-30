#nullable enable

using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace JetBlack.MessageBus.Common.Security.Cryptography
{
    internal static class CertUtils
    {
        private const string Prefix = "-----BEGIN CERTIFICATE-----";
        private const string Suffix = "-----END CERTIFICATE-----";

        public static X509Certificate2 FromFile(this string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var rawData = stream.Base64Decode(Prefix, Suffix);
                return new X509Certificate2(rawData);
            }
        }

        public static X509Certificate2 FromText(this string text)
        {
            using (var reader = new StringReader(text))
            {
                var rawData = reader.Base64Decode(Prefix, Suffix);
                return new X509Certificate2(rawData);
            }
        }
    }
}