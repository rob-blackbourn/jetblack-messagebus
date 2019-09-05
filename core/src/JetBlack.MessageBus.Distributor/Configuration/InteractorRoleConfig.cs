#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class InteractorRoleConfig
    {
        public List<Role>? Allow { get; set; }
        public List<Role>? Deny { get; set; }

        public InteractorRole ToInteractorRole(string host, string user)
        {
            var expandedHost = Environment.ExpandEnvironmentVariables(host);

            return new InteractorRole(
              expandedHost,
              user,
              Allow.Aggregate(Role.None, (aggregate, role) => aggregate | role),
              Deny.Aggregate(Role.None, (aggregate, role) => aggregate | role));
        }
    }
}
