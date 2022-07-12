using System.IO;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// An abstract client authenticator using connection strings.
    /// </summary>
    /// <remarks>
    /// A base class for client authenticators which use connection strings
    /// to pass credentials.
    ///
    /// In the case of a client which is acting as a proxy the
    /// authenticator is provided with the user the proxy is
    /// impersonating, and the host on which that client is running.
    /// </remarks>
    public abstract class ClientAuthenticator : IClientAuthenticator
    {
        /// <summary>
        /// Construct the client authenticator.
        /// </summary>
        /// <param name="impersonating">The optional user that the client is impersonating.</param>
        /// <param name="forwardedFor">The optional host for which the client is forwarding requests.</param>
        /// <param name="application">The name of the application.</param>
        public ClientAuthenticator(
            string? impersonating = null,
            string? forwardedFor = null,
            string? application = null)
        {
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
        }

        /// <inheritdoc />
        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            var connectionString = ToConnectionString();
            writer.Write(connectionString.ToString());
        }

        /// <inheritdoc />
        public string? Impersonating { get; }
        /// <inheritdoc />
        public string? ForwardedFor { get; }
        /// <inheritdoc />
        public string? Application { get; }

        /// <summary>
        /// Create a connection string.
        /// </summary>
        /// <returns></returns>
        protected abstract string ToConnectionString();
    }
}