using System;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// A request for authorization.
    /// </summary>
    public class AuthorizationRequest : Message, IEquatable<AuthorizationRequest>
    {
        /// <summary>
        /// Construct an authorization request.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="host">The host on which the client is requesting the subscription.</param>
        /// <param name="user">The user that is making the subscription.</param>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="topic">The name of the topic.</param>
        public AuthorizationRequest(Guid clientId, string host, string user, string feed, string topic)
            : base(MessageType.AuthorizationRequest)
        {
            ClientId = clientId;
            Host = host;
            User = user;
            Feed = feed;
            Topic = topic;
        }

        /// <summary>
        /// The client identifier.
        /// </summary>
        public Guid ClientId { get; }
        /// <summary>
        /// The host on which the client is requesting the subscription.
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// The user that is making the subscription.
        /// </summary>
        public string User { get; }
        /// <summary>
        /// The name of the feed.
        /// </summary>
        public string Feed { get; }
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string Topic { get; }

        /// <summary>
        /// Read the body of the message.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <returns>The authorization request.</returns>
        public static AuthorizationRequest ReadBody(DataReader reader)
        {
            var clientId = reader.ReadGuid();
            var host = reader.ReadString();
            var user = reader.ReadString();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            return new AuthorizationRequest(clientId, host, user, feed, topic);
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Test another request for equality.
        /// </summary>
        /// <param name="other">The other request.</param>
        /// <returns>True if the requests are equal.</returns>
        public bool Equals(AuthorizationRequest? other)
        {
            return other != null &&
                ClientId == other.ClientId &&
                Host == other.Host &&
                User == other.User &&
                Feed == other.Feed &&
                Topic == other.Topic;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as AuthorizationRequest);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
                ClientId.GetHashCode() ^
                Host.GetHashCode() ^
                User.GetHashCode() ^
                Feed.GetHashCode() ^
                Topic.GetHashCode();
        }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(ClientId)}={ClientId}" +
            $",{nameof(Host)}=\"{Host}\"" +
            $",{nameof(User)}=\"{User}\"" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"";
    }
}
