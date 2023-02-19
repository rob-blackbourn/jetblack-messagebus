using System;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The client authenticator to use with SSPI authentication.
    /// </summary>
    public class SspiClientAuthenticator : ClientAuthenticator
    {
        public SspiClientAuthenticator(string? domainName = null, string? userName = null)
        {
            DomainName = domainName ?? Environment.UserDomainName;
            UserName = userName ?? Environment.UserName;
        }

        public string UserName { get; }
        public string DomainName { get; }

        /// <inheritdoc />
        protected override string ToConnectionString()
        {
            return $"Username={DomainName}\\{UserName}";
        }
    }
}