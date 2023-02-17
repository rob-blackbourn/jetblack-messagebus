using System;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    [Flags]
    public enum Role
    {
        None = 0x00,
        Subscribe = 0x01,
        Publish = 0x02,
        Notify = 0x04,
        Authorize = 0x08,
        All = Subscribe | Publish | Notify | Authorize
    }
}