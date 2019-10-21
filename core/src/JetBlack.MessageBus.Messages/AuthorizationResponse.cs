#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class AuthorizationResponse : Message, IEquatable<AuthorizationResponse>
    {
        public AuthorizationResponse(Guid clientId, string feed, string topic, bool isAuthorizationRequired, HashSet<int>? entitlements)
            : base(MessageType.AuthorizationResponse)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAuthorizationRequired = isAuthorizationRequired;
            Entitlements = entitlements;
        }

        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsAuthorizationRequired { get; }
        public HashSet<int>? Entitlements { get; }

        public static AuthorizationResponse ReadBody(DataReader reader)
        {
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isAuthorizationRequired = reader.ReadBoolean();
            var entitlements = reader.ReadInt32HashSet();
            return new AuthorizationResponse(clientId, feed, topic, isAuthorizationRequired, entitlements);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(ClientId);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsAuthorizationRequired);
            writer.Write(Entitlements);
            return writer;
        }

        public bool Equals(AuthorizationResponse? other)
        {
            return other != null &&
                ClientId == other.ClientId &&
                Feed == other.Feed &&
                Topic == other.Topic &&
                IsAuthorizationRequired == other.IsAuthorizationRequired &&
                Entitlements.SequenceEqual(other.Entitlements);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AuthorizationResponse);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
                ClientId.GetHashCode() ^
                Feed.GetHashCode() ^
                Topic.GetHashCode() ^
                (Entitlements?.GetHashCode() ?? 0);
        }

        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(ClientId)}={ClientId}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(IsAuthorizationRequired)}={IsAuthorizationRequired}" +
            $",{nameof(Entitlements)}.Count={Entitlements?.Count ?? 0}";
    }
}
