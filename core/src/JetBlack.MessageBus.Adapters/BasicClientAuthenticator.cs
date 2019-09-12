#nullable enable

using System.IO;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    public class BasicClientAuthenticator : IClientAuthenticator
    {
        private readonly string _password;
        public BasicClientAuthenticator(string username, string password, string? impersonating = null, string? forwardedFor = null)
        {
            Username = username;
            _password = password;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
        }

        public string Username { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }

        public void Authenticate(Stream stream)
        {
            var writer = new DataWriter(stream);
            writer.Write(Username);
            writer.Write(_password);
            writer.Write(Impersonating);
            writer.Write(ForwardedFor);
        }

        public override string ToString() => $"{nameof(Username)}=\"{Username}\",{nameof(Impersonating)}=\"{Impersonating}\",{nameof(ForwardedFor)}=\"{ForwardedFor}\"";
    }
}