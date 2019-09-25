#nullable enable

using System.Text;

namespace JetBlack.MessageBus.Adapters
{
    public class TokenClientAuthenticator : ClientAuthenticator
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

        protected override string ToConnectionString()
        {
            var connectionString = new StringBuilder();
            connectionString.Append("Token=").Append(Token);
            if (!string.IsNullOrWhiteSpace(Impersonating))
                connectionString.Append(';').Append("Impersonating=").Append(Impersonating);
            if (!string.IsNullOrWhiteSpace(ForwardedFor))
                connectionString.Append(';').Append("ForwardedFor=").Append(ForwardedFor);
            return connectionString.ToString();
        }

        public override string ToString() =>
            $"{nameof(Token)}=\"{Token}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"";
    }
}