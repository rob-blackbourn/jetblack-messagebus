#nullable enable

using System.IO;
using System.Security.Principal;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public interface IAuthenticator
    {
        GenericIdentity Authenticate(Stream stream);
    }
}