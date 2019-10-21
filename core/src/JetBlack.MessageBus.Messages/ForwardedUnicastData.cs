#nullable enable

using System;
using System.Linq;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class ForwardedUnicastData : Message, IEquatable<ForwardedUnicastData>
    {
        public ForwardedUnicastData(string user, string host, Guid clientId, string feed, string topic, bool isImage, DataPacket[]? dataPackets)
            : base(MessageType.ForwardedUnicastData)
        {
            User = user;
            Host = host;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            DataPackets = dataPackets;
        }

        public string User { get; }
        public string Host { get; }
        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public DataPacket[]? DataPackets { get; }

        public static ForwardedUnicastData ReadBody(DataReader reader)
        {
            var user = reader.ReadString();
            var host = reader.ReadString();
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isImage = reader.ReadBoolean();
            var dataPackets = reader.ReadBinaryDataPacketArray();
            return new ForwardedUnicastData(user, host, clientId, feed, topic, isImage, dataPackets);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(User);
            writer.Write(Host);
            writer.Write(ClientId);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsImage);
            writer.Write(DataPackets);
            return writer;
        }

        public bool Equals(ForwardedUnicastData? other)
        {
            return other != null &&
                User == other.User &&
                Host == other.Host &&
                ClientId == other.ClientId &&
                Feed == other.Feed &&
                Topic == other.Topic &&
              IsImage == other.IsImage &&
              (
                (DataPackets == null && other.DataPackets == null) ||
                (DataPackets != null && other.DataPackets != null && DataPackets.SequenceEqual(other.DataPackets))
              );
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ForwardedUnicastData);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              User.GetHashCode() ^
              Host.GetHashCode() ^
              ClientId.GetHashCode() ^
              Feed.GetHashCode() ^
              Topic.GetHashCode() ^
              IsImage.GetHashCode() ^
              (DataPackets?.GetHashCode() ?? 0);
        }

        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(User)}=\"{User}\"" +
            $",{nameof(Host)}=\"{Host}\"" +
            $",{nameof(ClientId)}={ClientId}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(IsImage)}={IsImage}" +
            $",{nameof(DataPackets)}.Length={DataPackets?.Length}";
    }
}
