using System.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    /// <summary>
    /// The abstract client authenticator.
    /// </summary>
    /// <remarks>
    /// In the case of a client which is acting as a proxy the
    /// authenticator is provided with the user the proxy is
    /// impersonating, and the host on which that client is running.
    /// </remarks>
    public interface IClientAuthenticator
    {
        /// <summary>
        /// Authenticate a client.
        /// </summary>
        /// <param name="stream"></param>
        void Authenticate(Stream stream);

        /// <summary>
        /// The user that the client is impersonating or null.
        /// </summary>
        string? Impersonating { get; }
        /// <summary>
        /// The host that the client is forwarding requests for or null.
        /// </summary>
        string? ForwardedFor { get; }
        /// <summary>
        /// The name of the application.
        /// </summary>
        string? Application { get; }
    }
}