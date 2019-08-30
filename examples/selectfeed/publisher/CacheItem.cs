#nullable enable

using System;
using System.Collections.Generic;

namespace publisher
{
    public class CacheItem
    {
        public Dictionary<Guid, bool> ClientStates { get; } = new Dictionary<Guid, bool>();
        public Dictionary<string, object>? Data { get; set; }
    }
}
