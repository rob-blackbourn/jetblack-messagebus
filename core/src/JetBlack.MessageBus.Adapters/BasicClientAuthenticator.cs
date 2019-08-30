#nullable enable

using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    public class BasicClientAuthenticator : IClientAuthenticator
    {
        private readonly string _password;
        public BasicClientAuthenticator(string username, string password)
        {
            Username = username;
            _password = password;
        }

        public string Username { get; }

        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            writer.Write(Username);
            writer.Write(_password);
        }
    }
}