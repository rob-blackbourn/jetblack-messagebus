using System.Collections.Generic;
using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication
{
    public class PermissionConfig
    {
        public List<Role>? Allow { get; set; }
        public List<Role>? Deny { get; set; }
    }
}