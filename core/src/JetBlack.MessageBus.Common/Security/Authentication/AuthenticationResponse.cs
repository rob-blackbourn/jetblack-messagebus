using System.Collections.Generic;
using System.Linq;

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
        /// <param name="feedRoles">The feed roles.</param>
        public AuthenticationResponse(
            string user,
            string method,
            string? impersonating,
            string? forwardedFor,
            string? application,
            Dictionary<string, Dictionary<string, Permission>>? feedRoles)
        {
            User = user;
            Method = method;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            Application = application;
            FeedRoles = feedRoles;
        }

        /// <summary>
        /// The user.
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// The authentication method.
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// If the authentication was a proxy, the impersonated user.
        /// </summary>
        public string? Impersonating { get; set; }
        /// <summary>
        /// If the authentication was a proxy the originating host.
        /// </summary>
        public string? ForwardedFor { get; set; }
        /// <summary>
        /// The name of the application.
        /// </summary>
        public string? Application { get; set; }
        /// <summary>
        /// The available roles for given feeds.
        ///
        /// The first dictionary is keyed by feed, and the second
        /// is keyed by a host regular expression.
        /// </summary>
        public Dictionary<string, Dictionary<string, Permission>>? FeedRoles { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var feedRoles = FeedRoles == null
                ? "<null>"
                : "{" + string.Join(
                    ", ",
                    FeedRoles.Select(
                        x => $"{x.Key}: {string.Join(", ", x.Value.ToString())}"
                    )) + "}";

            return 
                $"{nameof(User)}=\"{User}\"" +
                $",{nameof(Method)}=\"{Method}\"" +
                $",{nameof(Impersonating)}=\"{Impersonating}\"" +
                $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
                $",{nameof(Application)}=\"{Application}\"" +
                $",{nameof(FeedRoles)}={feedRoles}";
        }
    }
}