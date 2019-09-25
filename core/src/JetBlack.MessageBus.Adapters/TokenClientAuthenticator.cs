#nullable enable

using System.Text;

namespace JetBlack.MessageBus.Adapters
{
    public class TokenClientAuthenticator : ClientAuthenticator
    {
        public TokenClientAuthenticator(
            string token,
            string? impersonating = null,
            string? forwardedFor = null,
            string? application = null)
        {
            Token = token;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
        }

        public string Token { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }
        public string? Application { get; }

        protected override string ToConnectionString()
        {
            var connectionString = new StringBuilder();
            connectionString.Append("Token=").Append(Token);
            if (!string.IsNullOrWhiteSpace(Impersonating))
                connectionString.Append(';').Append("Impersonating=").Append(Impersonating);
            if (!string.IsNullOrWhiteSpace(ForwardedFor))
                connectionString.Append(';').Append("ForwardedFor=").Append(ForwardedFor);
            if (!string.IsNullOrWhiteSpace(Application))
                connectionString.Append(';').Append("Application=").Append(Application);
            return connectionString.ToString();
        }

        public override string ToString() =>
            $"{nameof(Token)}=\"{Token}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}