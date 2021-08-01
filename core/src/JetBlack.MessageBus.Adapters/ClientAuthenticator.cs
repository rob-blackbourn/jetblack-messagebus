#nullable enable

using System.IO;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The abstract client authenticator.
    /// </summary>
    /// <remarks>
    /// In the case of a client which is acting as a proxy the
    /// authenticator is provided with the user the proxy is
    /// impersonating, and the host on which that client is running.
    /// </remarks>
    public abstract class ClientAuthenticator
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

        /// <summary>
        /// Authenticate a client.
        /// </summary>
        /// <param name="stream"></param>
        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            var connectionString = ToConnectionString();
            writer.Write(connectionString.ToString());
        }

        /// <summary>
        /// The user that the client is impersonating or null.
        /// </summary>
        public string? Impersonating { get; }
        /// <summary>
        /// The host that the client is forwarding requests for or null.
        /// </summary>
        public string? ForwardedFor { get; }
        /// <summary>
        /// The name of the application.
        /// </summary>
        public string? Application { get; }

        /// <summary>
        /// Create a connection string.
        /// </summary>
        /// <returns></returns>
        protected abstract string ToConnectionString();
    }
}