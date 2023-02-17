using System.Collections.Generic;

using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Distributor.Roles
{
    public class RoleManager
    {
        private readonly IDictionary<string, IDictionary<Role, bool>> _feedDecision = new Dictionary<string, IDictionary<Role, bool>>();

        public RoleManager(DistributorRole distributorPermission, string host, string user, string? impersonating, string? forwardedFor)
        {
            DistributorRole = distributorPermission;
            Host = host;
            User = user;
            Impersonating = string.IsNullOrWhiteSpace(impersonating) ? null : impersonating;
            ForwardedFor = string.IsNullOrWhiteSpace(forwardedFor) ? null : forwardedFor;
        }

        public DistributorRole DistributorRole { get; }
        public string User { get; }
        public string Host { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }

        public bool HasRole(string feed, Role role)
        {
            // Check the cache .
            if (!_feedDecision.TryGetValue(feed, out var roleDecision))
                _feedDecision.Add(feed, roleDecision = new Dictionary<Role, bool>());
            if (roleDecision.TryGetValue(role, out var decision))
                return decision;

            decision = DistributorRole.HasRole(HostForFeed(feed), UserForFeed(feed), feed, role);

            // Cache the decision;
            roleDecision.Add(role, decision);

            return decision;
        }

        public bool IsAuthorizationRequired(string feed)
        {
            return DistributorRole.IsAuthorizationRequiredForFeed(feed);
        }

        public bool IsImpersonationAllowed(string feed)
        {
            return DistributorRole.IsImpersonationAllowedForFeed(feed);
        }

        public bool IsProxyAllowed(string feed)
        {
            return DistributorRole.IsProxyAllowedForFeed(feed);
        }

        public string UserForFeed(string feed)
        {
            return IsImpersonationAllowed(feed)
                ? Impersonating ?? User
                : User;
        }

        public string HostForFeed(string feed)
        {
            return IsProxyAllowed(feed)
                ? (ForwardedFor ?? Host)
                : Host;
        }

    }
}
