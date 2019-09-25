#nullable enable

using System.Text;

namespace JetBlack.MessageBus.Adapters
{
    public class BasicClientAuthenticator : ClientAuthenticator
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

        protected override string ToConnectionString()
        {
            var connectionString = new StringBuilder();
            connectionString.Append("Username=").Append(Username)
                .Append(';').Append("Password=").Append(_password);
            if (!string.IsNullOrWhiteSpace(Impersonating))
                connectionString.Append(';').Append("Impersonating=").Append(Impersonating);
            if (!string.IsNullOrWhiteSpace(ForwardedFor))
                connectionString.Append(';').Append("ForwardedFor=").Append(ForwardedFor);
            return connectionString.ToString();
        }

        public override string ToString() =>
            $"{nameof(Username)}=\"{Username}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"";
    }
}