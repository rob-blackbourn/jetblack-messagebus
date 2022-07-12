using System;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// The message sent when a client requests a subscription to a feed and topic.
    /// </summary>
    public class SubscriptionRequest : Message, IEquatable<SubscriptionRequest>
    {
        /// <summary>
        /// Construct the message.
        /// </summary>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="topic">The name of the topic.</param>
        /// <param name="isAdd">If true the subscription is being added, otherwise it is being removed.</param>
        public SubscriptionRequest(string feed, string topic, bool isAdd)
            : base(MessageType.SubscriptionRequest)
        {
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        /// <summary>
        /// The name of the feed.
        /// </summary>
        public string Feed { get; }
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string Topic { get; }
        /// <summary>
        /// If true the subscription is being added, otherwise it is being removed.
        /// </summary>
        public bool IsAdd { get; }

        /// <summary>
        /// Read the body of the message.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The message.</returns>
        public static SubscriptionRequest ReadBody(DataReader reader)
        {
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isAdd = reader.ReadBoolean();
            return new SubscriptionRequest(feed, topic, isAdd);
        }

        /// <inheritdoc />
        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(IsAdd);
            return writer;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The message to test against.</param>
        /// <returns>True if the messages were equal.</returns>
        public bool Equals(SubscriptionRequest? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
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
            return Equals(obj as SubscriptionRequest);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              (Feed ?? "").GetHashCode() ^
              (Topic ?? "").GetHashCode() ^
              IsAdd.GetHashCode();
        }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(IsAdd)}={IsAdd}";
    }
}
