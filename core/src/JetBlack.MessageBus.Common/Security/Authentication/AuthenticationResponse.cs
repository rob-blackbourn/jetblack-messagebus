#nullable enable

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    /// <summary>
    /// A successfull response to authentication.
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// Construct an authentication response.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="method">The method of authentication used.</param>
        /// <param name="impersonating">If the authentication was a proxy, the impersonated user.</param>
        /// <param name="forwardedFor">If the authentication was a proxy the originating host.</param>
        /// <param name="application">The name of the application.</param>
        public AuthenticationResponse(string user, string method, string? impersonating, string? forwardedFor, string? application)
        {
            User = user;
            Method = method;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
        }

        /// <summary>
        /// The user.
        /// </summary>
        public string User { get; }
        /// <summary>
        /// The authentication method.
        /// </summary>
        public string Method { get; }
        /// <summary>
        /// If the authentication was a proxy, the impersonated user.
        /// </summary>
        public string? Impersonating { get; }
        /// <summary>
        /// If the authentication was a proxy the originating host.
        /// </summary>
        public string? ForwardedFor { get; }
        /// <summary>
        /// The name of the application.
        /// </summary>
        public string? Application { get; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(User)}=\"{User}\"" +
            $",{nameof(Method)}=\"{Method}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Application)}=\"{Application}\"";
    }
}