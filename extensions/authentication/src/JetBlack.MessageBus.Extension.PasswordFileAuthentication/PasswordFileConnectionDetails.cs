#nullable enable

using System;
using System.Linq;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication
{
    public class PasswordFileConnectionDetails
    {
        private PasswordFileConnectionDetails(
            string username,
            string password,
            string? impersonating,
            string? forwardedFor,
            string? application)
        {
            Username = username;
            Password = password;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
        }

        public string Username { get; }
        public string Password { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }
        public string? Application { get; }

        public static PasswordFileConnectionDetails Parse(string connectionString)
        {
            var dict = connectionString.Split(new char[] {';'})
                .Select(part => part.Split(new char[] {'='}, 2))
                .ToDictionary(key => key[0], value => value[1]);

            if (!dict.TryGetValue("Username", out var username))
                throw new ArgumentException("Failed to find \"Username\".");
            if (!dict.TryGetValue("Password", out var password))
                throw new ArgumentException("Failed to find \"Password\"");
            dict.TryGetValue("Impersonating", out var impersonating);
            dict.TryGetValue("ForwardedFor", out var forwardedFor);
            dict.TryGetValue("Application", out var application);

            return new PasswordFileConnectionDetails(
                username,
                password,
                impersonating,
                forwardedFor,
                application);
        }

        public override string ToString() =>
            $"{nameof(Username)}=\"{Username}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}