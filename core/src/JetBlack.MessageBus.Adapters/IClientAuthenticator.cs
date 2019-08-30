#nullable enable

using System.IO;

namespace JetBlack.MessageBus.Adapters
{
    public interface IClientAuthenticator
    {
        void Authenticate(Stream stream);
    }
}