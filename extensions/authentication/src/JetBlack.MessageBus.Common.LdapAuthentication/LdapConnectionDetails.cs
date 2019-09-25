#nullable enable

using System;
using System.Linq;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class LdapConnectionDetails
    {
        private LdapConnectionDetails(string username, string password, string? impersonating, string? forwardedFor)
        {
            Username = username;
            Password = password;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
        }

        public string Username { get; }
        public string Password { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }

        public static LdapConnectionDetails Parse(string connectionString)
        {
            var dict = connectionString.Split(';')
                .Select(part => part.Split('=', 2))
                .ToDictionary(key => key[0], value => value[1]);

            if (!dict.TryGetValue("Username", out var username))
                throw new ArgumentException("Failed to find \"Username\".");
            if (!dict.TryGetValue("Password", out var password))
                throw new ArgumentException("Failed to find \"Password\"");
            dict.TryGetValue("Impersonating", out var impersonating);
            dict.TryGetValue("ForwardedFor", out var forwardedFor);

            return new LdapConnectionDetails(username, password, impersonating, forwardedFor);
        }

        public override string ToString() =>
            $"{nameof(Username)}=\"{Username}\"" +
            ",{nameof(Impersonating)}=\"{Impersonating}\"" +
            ",{nameof(ForwardedFor)}=\"{ForwardedFor}\"";
    }
}