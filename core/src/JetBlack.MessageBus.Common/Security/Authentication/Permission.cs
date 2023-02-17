using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class Permission
    {
        public List<Role>? Allow { get; set; }
        public List<Role>? Deny { get; set; }
    }
}