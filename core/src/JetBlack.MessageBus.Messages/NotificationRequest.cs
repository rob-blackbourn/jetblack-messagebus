#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// The message sent when a client requests notifications of subscriptions to a topic on a feed.
    /// </summary>
    public class NotificationRequest : Message, IEquatable<NotificationRequest>
    {
        /// <summary>
        /// Construct the message.
        /// </summary>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="isAdd">If true the request is being added, otherwise it is being removed.</param>
        public NotificationRequest(string feed, bool isAdd)
            : base(MessageType.NotificationRequest)
        {
            Feed = feed;
            IsAdd = isAdd;
        }

        /// <summary>
        /// The name of the feed.
        /// </summary>
        public string Feed { get; }
        /// <summary>
        /// If true the request is being added, otherwise it is being removed.
        /// </summary>
        public bool IsAdd { get; }

        /// <summary>
        /// Read the message body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The message.</returns>
        public static NotificationRequest ReadBody(DataReader reader)
        {
            var feed = reader.ReadString();
            var isAdd = reader.ReadBoolean();
            return new NotificationRequest(feed, isAdd);
        }

        /// <inheritdoc />
        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(Feed);
            writer.Write(IsAdd);
            return writer;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The message to test against.</param>
        /// <returns>True iof the messages are equal.</returns>
        public bool Equals(NotificationRequest? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
              Feed == other.Feed &&
              IsAdd == other.IsAdd;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as NotificationRequest);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              (Feed ?? "").GetHashCode() ^
              IsAdd.GetHashCode();
        }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(IsAdd)}={IsAdd}";
    }
}
