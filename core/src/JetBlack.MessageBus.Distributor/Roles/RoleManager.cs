#nullable enable

using System.Collections.Generic;

namespace JetBlack.MessageBus.Distributor.Roles
{
    public class RoleManager
    {
        private readonly DistributorRole _distributorRole;
        private readonly string _host;
        private readonly string _user;
        private readonly IDictionary<string, IDictionary<Role, bool>> _feedDecision = new Dictionary<string, IDictionary<Role, bool>>();

        public RoleManager(DistributorRole distributorPermission, string host, string user)
        {
            _distributorRole = distributorPermission;
            _host = host;
            _user = user;
        }

        public bool HasRole(string feed, Role role)
        {
            // Check the cache .
            if (!_feedDecision.TryGetValue(feed, out var roleDecision))
                _feedDecision.Add(feed, roleDecision = new Dictionary<Role, bool>());
            if (roleDecision.TryGetValue(role, out var decision))
                return decision;

            decision = _distributorRole.HasRole(_host, _user, feed, role);

            // Cache the decision;
            roleDecision.Add(role, decision);

            return decision;
        }
    }
}
