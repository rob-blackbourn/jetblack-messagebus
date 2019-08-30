#nullable enable

using System;
using System.Collections.Generic;

namespace JetBlack.MessageBus.Distributor
{
    public class AuthorizationInfo
    {
        public AuthorizationInfo(bool isAuthorizationRequired, ISet<Guid> entitlements)
        {
            IsAuthorizationRequired = isAuthorizationRequired;
            Entitlements = entitlements;
        }

        public bool IsAuthorizationRequired { get; private set; }
        public ISet<Guid> Entitlements { get; private set; }
    }
}
