#nullable enable

using System;
using System.Linq;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class JwtConnectionDetails
    {
        private JwtConnectionDetails(string token, string? impersonating, string? forwardedFor)
        {
            Token = token;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
        }

        public string Token { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }

        public static JwtConnectionDetails Parse(string connectionString)
        {
            var dict = connectionString.Split(';')
                .Select(part => part.Split('=', 2))
                .ToDictionary(key => key[0], value => value[1]);

            if (!dict.TryGetValue("Token", out var token))
                throw new ArgumentException("Failed to find \"Token\".");
            dict.TryGetValue("Impersonating", out var impersonating);
            dict.TryGetValue("ForwardedFor", out var forwardedFor);

            return new JwtConnectionDetails(token, impersonating, forwardedFor);
        }

        public override string ToString() =>
            $"{nameof(Token)}=\"{Token}\"" +
            ",{nameof(Impersonating)}=\"{Impersonating}\"" +
            ",{nameof(ForwardedFor)}=\"{ForwardedFor}\"";
    }
}