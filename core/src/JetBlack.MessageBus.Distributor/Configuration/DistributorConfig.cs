#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Common.Configuration;
using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class DistributorConfig
    {
        public string? Address { get; set; }
        public int Port { get; set; }
        public TimeSpan HeartbeatInterval { get; set; }
        public SslConfig? SslConfig { get; set; }
        public PluginConfig? Authentication { get; set; }
        public List<Role>? Allow { get; set; }
        public List<Role>? Deny { get; set; }
        public bool IsAuthorizationRequired { get; set; }
        public Dictionary<string, FeedRoleConfig>? FeedRoles { get; set; }

        public DistributorRole ToDistributorRole()
        {
            return new DistributorRole(
                Allow.Aggregate(Role.None, (aggregate, role) => aggregate | role),
                Deny.Aggregate(Role.None, (aggregate, role) => aggregate | role),
                IsAuthorizationRequired,
                FeedRoles?.ToDictionary(x => x.Key, x => x.Value.ToFeedRole(x.Key)));
        }
    }
}