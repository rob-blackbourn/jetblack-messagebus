#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class AuthorizationRequest : Message, IEquatable<AuthorizationRequest>
    {
        public AuthorizationRequest(Guid clientId, string host, string user, string feed, string topic)
            : base(MessageType.AuthorizationRequest)
        {
            ClientId = clientId;
            Host = host;
            User = user;
            Feed = feed;
            Topic = topic;
        }

        public Guid ClientId { get; }
        public string Host { get; }
        public string User { get; }
        public string Feed { get; }
        public string Topic { get; }

        public static AuthorizationRequest ReadBody(DataReader reader)
        {
            var clientId = reader.ReadGuid();
            var host = reader.ReadString();
            var user = reader.ReadString();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            return new AuthorizationRequest(clientId, host, user, feed, topic);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(ClientId);
            writer.Write(Host);
            writer.Write(User);
            writer.Write(Feed);
            writer.Write(Topic);
            return writer;
        }

        public bool Equals(AuthorizationRequest? other)
        {
            return other != null &&
                ClientId == other.ClientId &&
                Host == other.Host &&
                User == other.User &&
                Feed == other.Feed &&
                Topic == other.Topic;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AuthorizationRequest);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
                ClientId.GetHashCode() ^
                Host.GetHashCode() ^
                User.GetHashCode() ^
                Feed.GetHashCode() ^
                Topic.GetHashCode();
        }

        public override string ToString()
        {
            return $"{base.ToString()}, ClientId={ClientId}, Host={Host}, User={User}, Feed={Feed}, Topic={Topic}";
        }
    }
}
