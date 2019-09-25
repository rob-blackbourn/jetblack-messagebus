#nullable enable

using System.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public interface IAuthenticator
    {
        string Method { get; }
        AuthenticationResponse Authenticate(Stream stream);
    }
}