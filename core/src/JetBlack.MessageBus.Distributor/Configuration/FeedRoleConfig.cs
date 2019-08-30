#nullable enable

using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class FeedRoleConfig
    {
        public List<Role>? Allow { get; set; }
        public List<Role>? Deny { get; set; }
        public bool IsAuthorized { get; set; }
        public Dictionary<string, Dictionary<string, InteractorRoleConfig>?>? InteractorRoles { get; set; }

        public FeedRole ToFeedRole(string feed)
        {
            return new FeedRole(
                feed,
                Allow.Aggregate(Role.None, (aggregate, role) => aggregate | role),
                Deny.Aggregate(Role.None, (aggregate, role) => aggregate | role),
                IsAuthorized,
                InteractorRoles?.SelectMany(x => x.Value.Select(y => y.Value.ToInteractorRole(x.Key, y.Key)))?.ToList());
        }
    }
}
