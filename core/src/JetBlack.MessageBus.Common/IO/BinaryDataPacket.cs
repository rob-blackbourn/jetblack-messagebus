#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.IO
{
    public class BinaryDataPacket : IEquatable<BinaryDataPacket>
    {
        public BinaryDataPacket(HashSet<int>? entitlements, byte[]? body)
        {
            Entitlements = entitlements;
            Data = body;
        }

        public HashSet<int>? Entitlements { get; }
        public byte[]? Data { get; }

        public bool Equals(BinaryDataPacket? other)
        {
            return other != null &&
                (
                    (Entitlements == null && other.Entitlements == null) ||
                    (Entitlements != null && Entitlements.SetEquals(other.Entitlements))
                ) &&
                (
                    (Data == null && other.Data == null) ||
                    (Data != null && Data.SequenceEqual(other.Data))
                );
        }

        public bool IsAuthorized(ISet<int> allEntitlements)
        {
            return Entitlements != null && allEntitlements.IsSupersetOf(Entitlements);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BinaryDataPacket);
        }

        public override int GetHashCode()
        {
            return (Entitlements?.GetHashCode() ?? 0) ^ (Data?.GetHashCode() ?? 0);
        }

        public override string ToString() => $"{nameof(Entitlements)}.Count={Entitlements?.Count ?? 0}, {nameof(Data)}.Length={Data?.Length ?? 0}";
    }
}