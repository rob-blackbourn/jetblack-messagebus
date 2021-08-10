#nullable enable

using System.Text;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The base class for client authenticators.
    /// </summary>
    public class BasicClientAuthenticator : ClientAuthenticator
    {
        private readonly string _password;

        /// <summary>
        /// Construct a basic client authenticator.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="impersonating">The optional user that the client is impersonating.</param>
        /// <param name="forwardedFor">The optional host for which the client is forwarding requests.</param>
        /// <param name="application">The name of the application.</param>
        public BasicClientAuthenticator(
            string username,
            string password,
            string? impersonating = null,
            string? forwardedFor = null,
            string? application = null)
            : base(impersonating, forwardedFor, application)
        {
            Username = username;
            _password = password;
        }

        public string Username { get; }

        protected override string ToConnectionString()
        {
            var connectionString = new StringBuilder();
            connectionString.Append("Username=").Append(Username)
                .Append(';').Append("Password=").Append(_password);
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
            $"{nameof(Username)}=\"{Username}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}