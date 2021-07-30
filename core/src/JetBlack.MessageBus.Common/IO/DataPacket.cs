#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.IO
{
    /// <summary>
    /// A packet of data.
    /// </summary>
    public class DataPacket : IEquatable<DataPacket>
    {
        /// <summary>
        /// Construct the data packet.
        /// </summary>
        /// <param name="entitlements">An optional set of entitlements.</param>
        /// <param name="data">The data.</param>
        public DataPacket(ISet<int>? entitlements, byte[]? data)
        {
            Entitlements = entitlements;
            Data = data;
        }

        /// <summary>
        /// An optional set of entitlements.
        /// </summary>
        public ISet<int>? Entitlements { get; }
        /// <summary>
        /// The data.
        /// </summary>
        public byte[]? Data { get; }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The data packet to check.</param>
        /// <returns>true if the data packets are the same; otherwise false.</returns>
        public bool Equals(DataPacket? other)
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

        /// <summary>
        /// Check is the daata packet is authorised.
        /// </summary>
        /// <param name="allEntitlements">The entitlements that must be satisfied.</param>
        /// <returns>true if the entitlements were satisfied, otherwise false.</returns>
        public bool IsAuthorized(ISet<int> allEntitlements)
        {
            return Entitlements != null && allEntitlements.IsSupersetOf(Entitlements);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as DataPacket);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Entitlements?.GetHashCode() ?? 0) ^ (Data?.GetHashCode() ?? 0);
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Entitlements)}.Count={Entitlements?.Count ?? 0}, {nameof(Data)}.Length={Data?.Length ?? 0}";
    }
}