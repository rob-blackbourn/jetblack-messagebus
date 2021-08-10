#nullable enable

using System.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    /// <summary>
    /// The interface authenticators must implement.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// The authentication method.
        /// </summary>
        string Method { get; }
        /// <summary>
        /// Authenticate the client.
        /// </summary>
        /// <param name="stream">The stream to use for authentication.</param>
        /// <returns>The authentication response.</returns>
        /// <exception cref="System.Security.SecurityException">When authentication fails.</exception>
        AuthenticationResponse Authenticate(Stream stream);
    }
}