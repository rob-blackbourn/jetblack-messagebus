#nullable enable

using System.Text;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// A token based authenticator.
    /// </summary>
    public class TokenClientAuthenticator : ClientAuthenticator
    {
        /// <summary>
        /// Construct the token based authenticator.
        /// </summary>
        /// <param name="token">The token to authenticate with.</param>
        /// <param name="impersonating"></param>
        /// <param name="forwardedFor"></param>
        /// <param name="application"></param>
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

        /// <summary>
        /// The token used for authentication.
        /// </summary>
        public string Token { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }
        public string? Application { get; }

        /// <inheritdoc />
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