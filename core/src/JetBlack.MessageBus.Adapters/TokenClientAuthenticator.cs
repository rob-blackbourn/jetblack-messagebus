#nullable enable

using System.IO;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    public class TokenClientAuthenticator : IClientAuthenticator
    {
        public TokenClientAuthenticator(string token, string? impersonating = null, string? forwardedFor = null)
        {
            Token = token;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
        }

        public string Token { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }

        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            writer.Write(Token);
            writer.Write(Impersonating);
            writer.Write(ForwardedFor);
        }

        public override string ToString() => $"{nameof(Token)}=\"{Token}\",{nameof(Impersonating)}=\"{Impersonating}\",{nameof(ForwardedFor)}=\"{ForwardedFor}\"";
    }
}