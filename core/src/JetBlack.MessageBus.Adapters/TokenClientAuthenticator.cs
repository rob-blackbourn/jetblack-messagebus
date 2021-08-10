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
        /// <param name="impersonating">The optional user that the client is impersonating.</param>
        /// <param name="forwardedFor">The optional host for which the client is forwarding requests.</param>
        /// <param name="application">The name of the application.</param>
        public TokenClientAuthenticator(
            string token,
            string? impersonating = null,
            string? forwardedFor = null,
            string? application = null)
            : base(impersonating, forwardedFor, application)
        {
            Token = token;
        }

        /// <summary>
        /// The token used for authentication.
        /// </summary>
        public string Token { get; }

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

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{nameof(Token)}=\"{Token}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}