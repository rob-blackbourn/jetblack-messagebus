#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class ForwardedUnicastData : Message
    {
        public ForwardedUnicastData(string user, string host, Guid clientId, string feed, string topic, bool isImage, BinaryDataPacket[]? data)
            : base(MessageType.ForwardedUnicastData)
        {
            User = user;
            Host = host;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public string User { get; }
        public string Host { get; }
        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public BinaryDataPacket[]? Data { get; }

        public static ForwardedUnicastData ReadBody(DataReader reader)
        {
            var user = reader.ReadString();
            var host = reader.ReadString();
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isImage = reader.ReadBoolean();
            var data = reader.ReadBinaryDataPacketArray();
            return new ForwardedUnicastData(user, host, clientId, feed, topic, isImage, data);
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
            writer.Write(Data);
            return writer;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, User={User}, Host={Host}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsImage={IsImage}, Data={Data?.Length}";
        }
    }
}
