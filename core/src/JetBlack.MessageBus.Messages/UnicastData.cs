#nullable enable

using System;
using System.Linq;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class UnicastData : Message, IEquatable<UnicastData>
    {
        public UnicastData(Guid clientId, string feed, string topic, bool isImage, DataPacket[]? dataPackets)
            : base(MessageType.UnicastData)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            DataPackets = dataPackets;
        }

        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public DataPacket[]? DataPackets { get; }

        public static UnicastData ReadBody(DataReader reader)
        {
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isImage = reader.ReadBoolean();
            var dataPackets = reader.ReadBinaryDataPacketArray();
            return new UnicastData(clientId, feed, topic, isImage, dataPackets);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(ClientId);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsImage);
            writer.Write(DataPackets);
            return writer;
        }

        public bool Equals(UnicastData? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
              ClientId == other.ClientId &&
              Feed == other.Feed &&
              Topic == other.Topic &&
              IsImage == other.IsImage &&
              (
                (DataPackets == null && other.DataPackets == null) ||
                (DataPackets != null && other.DataPackets != null && DataPackets.SequenceEqual(other.DataPackets))
              );
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UnicastData);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              ClientId.GetHashCode() ^
              Feed.GetHashCode() ^
              Topic.GetHashCode() ^
              IsImage.GetHashCode() ^
              (DataPackets?.GetHashCode() ?? 0);

        }

        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(ClientId)}={ClientId}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(IsImage)}={IsImage}" +
            $",{nameof(DataPackets)}.Length={DataPackets?.Length}";
    }
}
