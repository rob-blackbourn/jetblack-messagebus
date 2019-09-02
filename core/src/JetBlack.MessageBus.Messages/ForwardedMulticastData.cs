#nullable enable

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class ForwardedMulticastData : Message
    {
        public ForwardedMulticastData(string user, string host, string feed, string topic, bool isImage, BinaryDataPacket[]? data)
            : base(MessageType.ForwardedMulticastData)
        {
            User = user;
            Host = host;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
        }

        public string User { get; }
        public string Host { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public BinaryDataPacket[]? Data { get; }

        public static ForwardedMulticastData ReadBody(DataReader reader)
        {
            var user = reader.ReadString();
            var host = reader.ReadString();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isImage = reader.ReadBoolean();
            var data = reader.ReadBinaryDataPacketArray();
            return new ForwardedMulticastData(user, host, feed, topic, isImage, data);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(User);
            writer.Write(Host);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsImage);
            writer.Write(Data);
            return writer;
        }

        public override string ToString() => $"{base.ToString()},{nameof(User)}=\"{User}\",{nameof(Host)}=\"{Host}\",{nameof(Feed)}=\"{Feed}\",{nameof(Topic)}=\"{Topic}\",{nameof(IsImage)}={IsImage},{nameof(Data)}.Length={Data?.Length}";
    }
}
