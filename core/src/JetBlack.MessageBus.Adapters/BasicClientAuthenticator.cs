#nullable enable

using System.Text;

namespace JetBlack.MessageBus.Adapters
{
    public class BasicClientAuthenticator : ClientAuthenticator
    {
        private readonly string _password;
        public BasicClientAuthenticator(
            string username,
            string password,
            string? impersonating = null,
            string? forwardedFor = null,
            string? application = null)
        {
            Username = username;
            _password = password;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
        }

        public string Username { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }
        public string? Application { get; }

        protected override string ToConnectionString()
        {
            var connectionString = new StringBuilder();
            connectionString.Append("Username=").Append(Username)
                .Append(';').Append("Password=").Append(_password);
            if (!string.IsNullOrWhiteSpace(Impersonating))
                connectionString.Append(';').Append("Impersonating=").Append(Impersonating);
            if (!string.IsNullOrWhiteSpace(ForwardedFor))
                connectionString.Append(';').Append("ForwardedFor=").Append(ForwardedFor);
            if (!string.IsNullOrWhiteSpace(Application))
                connectionString.Append(';').Append("Application=").Append(Application);
            return connectionString.ToString();
        }

        public override string ToString() =>
            $"{nameof(Username)}=\"{Username}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}