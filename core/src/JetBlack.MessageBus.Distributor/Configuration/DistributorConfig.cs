using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class DistributorConfig
    {
        public string? Address { get; set; }
        public int Port { get; set; }
        public TimeSpan HeartbeatInterval { get; set; }
        public SslConfig? Ssl { get; set; }
        public SspiConfig? Sspi { get; set; }
        public PrometheusConfig? Prometheus { get; set; }
        public PluginConfig? Authentication { get; set; }
        public PluginConfig? SspiAuthentication { get; set; }
        public List<Role>? Allow { get; set; }
        public List<Role>? Deny { get; set; }
        public bool IsAuthorizationRequired { get; set; }
        public bool IsImpersonationAllowed { get; set; }
        public bool IsProxyAllowed { get; set; }
        public bool UseJsonLogger { get; set; } = false;

        public DistributorRole ToDistributorRole()
        {
            return new DistributorRole(
                Allow?.Aggregate(Role.None, (aggregate, role) => aggregate | role) ?? Role.None,
                Deny?.Aggregate(Role.None, (aggregate, role) => aggregate | role) ?? Role.None,
                IsAuthorizationRequired,
                IsImpersonationAllowed,
                IsProxyAllowed);
        }

        public IPEndPoint ToIPEndPoint()
        {
            if (Address == null || !IPAddress.TryParse(Address, out var address))
                address = IPAddress.Any;
            return new IPEndPoint(address, Port);
        }
    }
}