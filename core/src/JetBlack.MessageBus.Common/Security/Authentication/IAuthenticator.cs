#nullable enable

using System.IO;
using System.Security.Principal;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public interface IAuthenticator
    {
        string Name { get; }
        GenericIdentity Authenticate(Stream stream);
    }
}