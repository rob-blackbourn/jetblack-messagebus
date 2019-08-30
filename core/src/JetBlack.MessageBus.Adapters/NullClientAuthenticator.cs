#nullable enable

using System.IO;

namespace JetBlack.MessageBus.Adapters
{
    public class NullClientAuthenticator : IClientAuthenticator
    {
        public void Authenticate(Stream stream)
        {
            // Nothing to do
        }
    }
}