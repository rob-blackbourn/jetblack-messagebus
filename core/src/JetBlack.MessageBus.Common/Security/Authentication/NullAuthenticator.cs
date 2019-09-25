#nullable enable

using System.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class NullAuthenticator : IAuthenticator
    {
        public NullAuthenticator(string[] args)
        {
            // Nothing to do here.
        }

        public string Method => "NULL";

        public AuthenticationResponse Authenticate(Stream stream)
        {
            // Always true.
            return new AuthenticationResponse("nobody", Method, null, null, null);
        }
    }
}