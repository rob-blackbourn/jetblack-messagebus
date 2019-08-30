#nullable enable

using System;
using System.Linq;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class UnicastData : Message, IEquatable<UnicastData>
    {
        public UnicastData(Guid clientId, string feed, string topic, bool isImage, BinaryDataPacket[]? data)
            : base(MessageType.UnicastData)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public BinaryDataPacket[]? Data { get; }

        public static UnicastData ReadBody(DataReader reader)
        {
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isImage = reader.ReadBoolean();
            var data = reader.ReadBinaryDataPacketArray();
            return new UnicastData(clientId, feed, topic, isImage, data);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(ClientId);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsImage);
            writer.Write(Data);
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
                (Data == null && other.Data == null) ||
                (Data != null && other.Data != null && Data.SequenceEqual(other.Data))
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
              (Data?.GetHashCode() ?? 0);

        }

        public override string ToString()
        {
            return $"{base.ToString()}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsImage={IsImage}, Data={Data}";
        }
    }
}
