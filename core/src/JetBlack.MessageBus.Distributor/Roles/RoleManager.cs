using System.Collections.Generic;
using System.Linq;

using JetBlack.MessageBus.Common;
using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Distributor.Roles
{
    public class RoleManager
    {
        private readonly IDictionary<string, IDictionary<Role, bool>> _feedDecision = new Dictionary<string, IDictionary<Role, bool>>();

        public RoleManager(
            DistributorRole distributorPermission,
            string host,
            string user,
            string? impersonating,
            string? forwardedFor,
            Dictionary<string, Dictionary<string, Permission>>? feedPermissions)
        {
            DistributorRole = distributorPermission;
            Host = host;
            User = user;
            Impersonating = impersonating;
            ForwardedFor = forwardedFor;
            FeedPermissions = feedPermissions?.ToDictionary(
                feed => feed.Key,
                feed => feed.Value.Where(x => EffectiveHost.Glob(x.Key)).Select(x => x.Value).FirstOrDefault() ?? new Permission(Role.None, Role.All)
            ) ?? new Dictionary<string, Permission>();
        }

        public DistributorRole DistributorRole { get; }
        public string User { get; }
        public string Host { get; }
        public string? Impersonating { get; }
        public string? ForwardedFor { get; }
        public string EffectiveUser => DistributorRole.IsImpersonationAllowed ? Impersonating ?? User : User;
        public string EffectiveHost => DistributorRole.IsProxyAllowed ? ForwardedFor ?? Host : Host;
        public Dictionary<string, Permission> FeedPermissions { get; }

        public bool HasRole(string feed, Role role)
        {
            // Check the cache .
            if (!_feedDecision.TryGetValue(feed, out var roleDecision))
                _feedDecision.Add(feed, roleDecision = new Dictionary<Role, bool>());
            if (roleDecision.TryGetValue(role, out var decision))
                return decision;

            if (DistributorRole.Allow.HasFlag(role))
                decision = true;
            if (DistributorRole.Deny.HasFlag(role))
                decision = false;
                
            if (FeedPermissions != null && FeedPermissions.TryGetValue(feed, out var permission))
            {
                if (permission.Allow.HasFlag(role))
                    decision = true;

                if (permission.Deny.HasFlag(role))
                    decision = false;
            }

            // Cache the decision;
            roleDecision.Add(role, decision);

            return decision;
        }
    }
}
