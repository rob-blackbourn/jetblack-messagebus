using System;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The client authenticator to use with SSPI authentication.
    /// </summary>
    public class SspiClientAuthenticator : ClientAuthenticator
    {
        public SspiClientAuthenticator(string? userName = null)
        {
            UserName = userName != null
                ? userName
                : $"{Environment.UserDomainName}\\{Environment.UserName}";
        }

        public string UserName { get; }

        /// <inheritdoc />
        protected override string ToConnectionString()
        {
            return $"Username={UserName}";
        }
    }
}