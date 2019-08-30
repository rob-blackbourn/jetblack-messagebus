#nullable enable
using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class SubscriptionRequest : Message, IEquatable<SubscriptionRequest>
    {
        public SubscriptionRequest(string feed, string topic, bool isAdd)
            : base(MessageType.SubscriptionRequest)
        {
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        public string Feed { get; }
        public string Topic { get; }
        public bool IsAdd { get; }

        public static SubscriptionRequest ReadBody(DataReader reader)
        {
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isAdd = reader.ReadBoolean();
            return new SubscriptionRequest(feed, topic, isAdd);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsAdd);
            return writer;
        }

        public bool Equals(SubscriptionRequest? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
              Feed == other.Feed &&
              Topic == other.Topic &&
              IsAdd == other.IsAdd;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SubscriptionRequest);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              (Feed ?? "").GetHashCode() ^
              (Topic ?? "").GetHashCode() ^
              IsAdd.GetHashCode();
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Feed={Feed}, Topic={Topic}, IsAdd={IsAdd}";
        }
    }
}
