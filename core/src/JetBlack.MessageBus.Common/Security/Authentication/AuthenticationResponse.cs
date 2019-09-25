#nullable enable

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class AuthenticationResponse
    {
        public AuthenticationResponse(string user, string method, string? impersonating, string? forwardedFor, string? application)
        {
            User = user;
            Method = method;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
        }

        public string User { get; }
        public string Method { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }
        public string? Application { get; }

        public override string ToString() =>
            $"{nameof(User)}=\"{User}\"" +
            $",{nameof(Method)}=\"{Method}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}