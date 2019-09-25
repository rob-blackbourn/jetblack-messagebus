#nullable enable

using System.IO;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    public abstract class ClientAuthenticator
    {
        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            var connectionString = ToConnectionString();
            writer.Write(connectionString.ToString());
        }

        protected abstract string ToConnectionString();
    }
}