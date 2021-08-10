#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// A forwarded subscription requests. This is sent to clients that have requested notification
    /// on a feed.
    /// </summary>
    public class ForwardedSubscriptionRequest : Message, IEquatable<ForwardedSubscriptionRequest>
    {
        /// <summary>
        /// Construct the message.
        /// </summary>
        /// <param name="user">The user that requested the subscription.</param>
        /// <param name="host">The host of the user that requested the subscription.</param>
        /// <param name="clientId">The id of that client that requested the subscription.</param>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="topic">The name of the topic.</param>
        /// <param name="isAdd">If true the client is adding a subscription, otherwise it is a removal.</param>
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

        /// <summary>
        /// The user that requested the subscription.
        /// </summary>
        public string User { get; }
        /// <summary>
        /// The host of the user that requested the subscription.
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// The id of the client that made the subscription.
        /// </summary>
        public Guid ClientId { get; }
        /// <summary>
        /// The name of the feed.
        /// </summary>
        public string Feed { get; }
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string Topic { get; }
        /// <summary>
        /// If true the client is adding a subscription, otherwise it is a removal.
        /// </summary>
        public bool IsAdd { get; }

        /// <summary>
        /// Read the body of the message.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The message.</returns>
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

        /// <inheritdoc />
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

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The message to test.</param>
        /// <returns>True if the messages are equal.</returns>
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

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ForwardedSubscriptionRequest);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
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

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(User)}=\"{User}\"" +
            $",{nameof(Host)}=\"{Host}\"" +
            $",{nameof(ClientId)}={ClientId}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",IsAdd={IsAdd}";
    }
}
