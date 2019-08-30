#nullable enable

using System;
using System.Linq;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class MulticastData : Message, IEquatable<MulticastData>
    {
        public MulticastData(string feed, string topic, bool isImage, BinaryDataPacket[]? data)
            : base(MessageType.MulticastData)
        {
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public BinaryDataPacket[]? Data { get; }

        public static MulticastData ReadBody(DataReader reader)
        {
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isImage = reader.ReadBoolean();
            var data = reader.ReadBinaryDataPacketArray();
            return new MulticastData(feed, topic, isImage, data);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsImage);
            writer.Write(Data);
            return writer;
        }

        public bool Equals(MulticastData? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
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
            return Equals(obj as MulticastData);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              Feed.GetHashCode() ^
              Topic.GetHashCode() ^
              IsImage.GetHashCode() ^
              (Data?.GetHashCode() ?? 0);

        }

        public override string ToString()
        {
            return $"{base.ToString()}, Feed={Feed}, Topic={Topic}, IsImage={IsImage}, Data={Data}";
        }
    }
}
