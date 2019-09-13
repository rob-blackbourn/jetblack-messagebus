#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Distributor.Roles
{
    public class FeedRole
    {
        public FeedRole(string feed, Role allow, Role deny, bool isAuthorizationRequired, bool isImpersonationAllowed, bool isProxyAllowed, IEnumerable<InteractorRole>? interactorRoles)
        {
            Feed = feed;
            Allow = allow;
            Deny = deny;
            IsAuthorizationRequired = isAuthorizationRequired;
            IsImpersonationAllowed = isImpersonationAllowed;
            IsProxyAllowed = isProxyAllowed;
            InteractorRoles = (interactorRoles ?? new InteractorRole[0]).ToDictionary(x => new InteractorRole.Key(x.Host, x.User));
        }

        public string Feed { get; }
        public Role Allow { get; }
        public Role Deny { get; }
        public bool IsAuthorizationRequired { get; }
        public bool IsImpersonationAllowed { get; }
        public bool IsProxyAllowed { get; }
        public IReadOnlyDictionary<InteractorRole.Key, InteractorRole> InteractorRoles { get; }

        public bool HasRole(string host, string user, string feed, Role role, bool decision)
        {
            if (Allow.HasFlag(role))
                decision = true;

            if (Deny.HasFlag(role))
                decision = false;

            if (InteractorRoles.TryGetValue(new InteractorRole.Key(host, user), out var interactorRole))
                decision = interactorRole.HasRole(role, decision);
            else if (InteractorRoles.TryGetValue(new InteractorRole.Key("*", user), out interactorRole))
                decision = interactorRole.HasRole(role, decision);

            return decision;
        }

        public override string ToString() => $"{nameof(Feed)}={Feed},{nameof(Allow)}={Allow},{nameof(Deny)}={Deny},{nameof(IsAuthorizationRequired)}={IsAuthorizationRequired},{nameof(IsImpersonationAllowed)}={IsImpersonationAllowed},{nameof(IsProxyAllowed)}={IsProxyAllowed},{nameof(InteractorRoles)}=[{string.Join(", ", InteractorRoles.Values)}]";
    }
}
