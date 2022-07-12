using System.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    /// <summary>
    /// The null authenticator.
    /// </summary>
    public class NullAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Construct the authenticator.
        /// </summary>
        /// <param name="args">The null authenticator takes no arguments.</param>
        public NullAuthenticator(string[] args)
        {
            // Nothing to do here.
        }

        /// <summary>
        /// The method is "NULL".
        /// </summary>
        public string Method => "NULL";

        /// <summary>
        /// Authentication always succeeds with the user returned as "nobody".
        /// </summary>
        /// <param name="stream">The stream to authenticate on.</param>
        /// <returns>An authentication response for the user "nobody".</returns>
        public AuthenticationResponse Authenticate(Stream stream)
        {
            return new AuthenticationResponse("nobody", Method, null, null, null);
        }
    }
}