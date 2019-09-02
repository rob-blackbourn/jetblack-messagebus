#nullable enable

using System;
using System.Linq;

namespace JetBlack.MessageBus.Common.IO
{
    public class BinaryDataPacket : IEquatable<BinaryDataPacket>
    {
        public BinaryDataPacket(Guid header, byte[]? body)
        {
            Header = header;
            Body = body;
        }

        public Guid Header { get; }
        public byte[]? Body { get; }

        public bool Equals(BinaryDataPacket? other)
        {
            return other != null &&
                Header == other.Header &&
                (
                    (Body == null && other.Body == null) ||
                    (Body != null && Body.SequenceEqual(other.Body))
                );
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BinaryDataPacket);
        }

        public override int GetHashCode()
        {
            return Header.GetHashCode() ^ (Body?.GetHashCode() ?? 0);
        }

        public override string ToString() => $"{nameof(Header)}={Header}, {nameof(Body)}.Length={Body?.Length}";
    }
}