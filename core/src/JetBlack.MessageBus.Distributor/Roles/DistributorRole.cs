using System.Collections.Generic;

using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Distributor.Roles
{
    public class DistributorRole
    {
        public DistributorRole(Role allow, Role deny, bool isAuthorizationRequired, bool isImpersonationAllowed, bool isProxyAllowed)
        {
            Allow = allow;
            Deny = deny;
            IsAuthorizationRequired = isAuthorizationRequired;
            IsImpersonationAllowed = isImpersonationAllowed;
            IsProxyAllowed = isProxyAllowed;
        }

        public Role Allow { get; }
        public Role Deny { get; }
        public bool IsAuthorizationRequired { get; }
        public bool IsImpersonationAllowed { get; }
        public bool IsProxyAllowed { get; }

        public bool HasRole(Role role)
        {
            var decision = Allow.HasFlag(role);

            if (Deny.HasFlag(role))
                decision = false;

            return decision;
        }

        public override string ToString() => $"{nameof(Allow)}={Allow},{nameof(Deny)}={Deny},{nameof(IsAuthorizationRequired)}={IsAuthorizationRequired},{nameof(IsImpersonationAllowed)}={IsImpersonationAllowed},{nameof(IsProxyAllowed)}={IsProxyAllowed}";
    }
}
