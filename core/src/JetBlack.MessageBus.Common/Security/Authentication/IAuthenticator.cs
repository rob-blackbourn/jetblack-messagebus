#nullable enable

using System.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public interface IAuthenticator
    {
        string Name { get; }
        AuthenticationResponse Authenticate(Stream stream);
    }
}