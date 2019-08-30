#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class ForwardedSubscriptionRequest : Message, IEquatable<ForwardedSubscriptionRequest>
    {
        public ForwardedSubscriptionRequest(string user, string host, Guid clientId, string feed, string topic, bool isAdd)
            : base(MessageType.ForwardedSubscriptionRequest)
        {
            User = user;
            Host = host;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        public string User { get; }
        public string Host { get; }
        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsAdd { get; }

        public static ForwardedSubscriptionRequest ReadBody(DataReader reader)
        {
            var user = reader.ReadString();
            var host = reader.ReadString();
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isAdd = reader.ReadBoolean();
            return new ForwardedSubscriptionRequest(user, host, clientId, feed, topic, isAdd);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(User);
            writer.Write(Host);
            writer.Write(ClientId);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsAdd);
            return writer;
        }

        public bool Equals(ForwardedSubscriptionRequest? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
              User == other.User &&
              Host == other.Host &&
              ClientId == other.ClientId &&
              Feed == other.Feed &&
              Topic == other.Topic &&
              IsAdd == other.IsAdd;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ForwardedSubscriptionRequest);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
                User.GetHashCode() ^
                Host.GetHashCode() ^
                ClientId.GetHashCode() ^
                Feed.GetHashCode() ^
                Topic.GetHashCode() ^
                IsAdd.GetHashCode();
        }

        public override string ToString()
        {
            return $"{base.ToString()}, User={User}, Host={Host}, ClientId={ClientId}, Feed={Feed}, Topic={Topic}, IsAdd{IsAdd}";
        }
    }
}
