#nullable enable

using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.IO
{
    public class DataPacket
    {
        public DataPacket(HashSet<int>? entitlements, object? data)
        {
            Entitlements = entitlements;
            Data = data;
        }

        public HashSet<int>? Entitlements { get; }
        public object? Data { get; }

        public override string ToString() => $"{nameof(Entitlements)}.Count={Entitlements?.Count ?? 0},{nameof(Data)}={Data}";
    }
}
