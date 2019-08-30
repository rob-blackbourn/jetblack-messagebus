#nullable enable

using System.Collections.Generic;

namespace JetBlack.MessageBus.Distributor.Roles
{
    public class DistributorRole
    {
        public DistributorRole(Role allow, Role deny, bool isAuthorizationRequired, IReadOnlyDictionary<string, FeedRole>? feedRoles)
        {
            Allow = allow;
            Deny = deny;
            IsAuthorizationRequired = isAuthorizationRequired;
            FeedRoles = feedRoles ?? new Dictionary<string, FeedRole>();
        }

        public Role Allow { get; }
        public Role Deny { get; }
        public bool IsAuthorizationRequired { get; }
        public IReadOnlyDictionary<string, FeedRole> FeedRoles { get; }

        public bool HasRole(string host, string user, string feed, Role role)
        {
            var decision = Allow.HasFlag(role);

            if (Deny.HasFlag(role))
                decision = false;

            if (FeedRoles.TryGetValue(feed, out var feedPermission))
                decision = feedPermission.HasRole(host, user, feed, role, decision);

            return decision;
        }

        public bool IsAuthorizationRequiredForFeed(string feed)
        {
            if (FeedRoles.TryGetValue(feed, out var feedRole))
                return feedRole.IsAuthorizationRequired;
            return IsAuthorizationRequired;
        }

        public override string ToString() => $"{nameof(Allow)}={Allow},{nameof(Deny)}={Deny},{nameof(IsAuthorizationRequired)}={IsAuthorizationRequired},{nameof(FeedRoles)}=[{string.Join(", ", FeedRoles.Values)}]";
    }
}
