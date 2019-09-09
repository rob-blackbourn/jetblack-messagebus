#nullable enable

using System.IO;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    public class TokenClientAuthenticator : IClientAuthenticator
    {
        public TokenClientAuthenticator(string token)
        {
            Token = token;
        }

        public string Token { get; }

        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            writer.Write(Token);
        }

        public override string ToString() => $"{nameof(Token)}=\"{Token}\"";
    }
}