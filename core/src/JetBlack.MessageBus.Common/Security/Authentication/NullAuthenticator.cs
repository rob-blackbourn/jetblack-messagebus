#nullable enable

using System.IO;
using System.Security.Principal;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class NullAuthenticator : IAuthenticator
    {
        public NullAuthenticator(string[] args)
        {
            // Nothing to do here.
        }

        public string Name => "NULL";

        public GenericIdentity Authenticate(Stream stream)
        {
            // Always true.
            return new GenericIdentity("unknown", Name);
        }
    }
}