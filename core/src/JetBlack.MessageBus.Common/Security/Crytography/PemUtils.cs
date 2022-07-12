using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JetBlack.MessageBus.Common.Security.Cryptography
{
    internal static class PemUtils
    {
        internal static byte[] ExtractBytes(string text, string header, string footer)
        {
            string data = text;
            data = data.Replace(header, string.Empty);
            data = data.Replace(footer, string.Empty);

            return Convert.FromBase64String(data);
        }

        internal static byte[] Base64Decode(this Stream stream, string prefix, string suffix)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.Base64Decode(prefix, suffix);
            }
        }

        internal static byte[] Base64Decode(this TextReader reader, string prefix, string suffix)
        {
            return reader.Base64Decode(new Dictionary<string, string> { { prefix, suffix } }).Value;
        }

        internal static KeyValuePair<string, byte[]> Base64Decode(this Stream stream, IDictionary<string, string> prefixAndSuffix)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.Base64Decode(prefixAndSuffix);
            }
        }

        internal static KeyValuePair<string, byte[]> Base64Decode(this TextReader reader, IDictionary<string, string> prefixAndSuffix)
        {
            var content = new StringBuilder();
            string? line = null;
            string? prefix = null, suffix = null;

            // Skip to first line after prefix.
            while ((line = reader.ReadLine()) != null)
            {
                if (prefixAndSuffix.TryGetValue(line, out suffix))
                {
                    prefix = line;
                    break;
                }
            }

            if (prefix != null)
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == suffix)
                    {
                        var rawData = Convert.FromBase64String(content.ToString());
                        return new KeyValuePair<string, byte[]>(prefix, rawData);
                    }

                    content.Append(line);
                }
            }

            throw new FormatException();
        }
    }
}