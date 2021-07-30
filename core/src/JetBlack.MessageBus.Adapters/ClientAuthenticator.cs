#nullable enable

using System.IO;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The abstract client authenticator.
    /// </summary>
    public abstract class ClientAuthenticator
    {
        /// <summary>
        /// Authenticate a client.
        /// </summary>
        /// <param name="stream"></param>
        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            var connectionString = ToConnectionString();
            writer.Write(connectionString.ToString());
        }

        /// <summary>
        /// Create a connection string.
        /// </summary>
        /// <returns></returns>
        protected abstract string ToConnectionString();
    }
}