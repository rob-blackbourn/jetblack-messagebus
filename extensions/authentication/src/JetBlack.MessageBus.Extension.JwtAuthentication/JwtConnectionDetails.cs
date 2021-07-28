#nullable enable

using System;
using System.Linq;

namespace JetBlack.MessageBus.Extension.JwtAuthentication
{
    public class JwtConnectionDetails
    {
        private JwtConnectionDetails(
            string token,
            string? impersonating,
            string? forwardedFor,
            string? application)
        {
            Token = token;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
        }

        public string Token { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }
        public string? Application { get; }

        public static JwtConnectionDetails Parse(string connectionString)
        {
            var dict = connectionString.Split(';')
                .Select(part => part.Split('=', 2))
                .ToDictionary(key => key[0], value => value[1]);

            if (!dict.TryGetValue("Token", out var token))
                throw new ArgumentException("Failed to find \"Token\".");
            dict.TryGetValue("Impersonating", out var impersonating);
            dict.TryGetValue("ForwardedFor", out var forwardedFor);
            dict.TryGetValue("Application", out var application);

            return new JwtConnectionDetails(token, impersonating, forwardedFor, application);
        }

        public override string ToString() =>
            $"{nameof(Token)}=\"{Token}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}