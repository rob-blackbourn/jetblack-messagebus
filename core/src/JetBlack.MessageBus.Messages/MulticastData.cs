using System;
using System.Linq;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// Multicast data is sent when a client publishes data to subscribers.
    /// </summary>
    public class MulticastData : Message, IEquatable<MulticastData>
    {
        /// <summary>
        /// Construct the multicast message.
        /// </summary>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="topic">The name of the topic.</param>
        /// <param name="contentType">If true the data is considered complete.</param>
        /// <param name="dataPackets">The data packets.</param>
        public MulticastData(string feed, string topic, string contentType, DataPacket[]? dataPackets)
            : base(MessageType.MulticastData)
        {
            Feed = feed;
            Topic = topic;
            ContentType = contentType;
            DataPackets = dataPackets;
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
        /// If true the data is considered complete.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// The data packets.
        /// </summary>
        public DataPacket[]? DataPackets { get; }

        /// <summary>
        /// Read the message body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The message.</returns>
        public static MulticastData ReadBody(DataReader reader)
        {
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var contentType = reader.ReadString();
            var dataPackets = reader.ReadBinaryDataPacketArray();
            return new MulticastData(feed, topic, contentType, dataPackets);
        }

        /// <inheritdoc />
        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(ContentType);
            writer.Write(DataPackets);
            return writer;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The message to test against.</param>
        /// <returns>True if the messages were equal.</returns>
        public bool Equals(MulticastData? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
              Feed == other.Feed &&
              Topic == other.Topic &&
              string.Compare(ContentType, other.ContentType) == 0 &&
              (
                (DataPackets == null && other.DataPackets == null) ||
                (DataPackets != null && other.DataPackets != null && DataPackets.SequenceEqual(other.DataPackets))
              );
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as MulticastData);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              Feed.GetHashCode() ^
              Topic.GetHashCode() ^
              ContentType.GetHashCode() ^
              (DataPackets?.GetHashCode() ?? 0);
        }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(ContentType)}={ContentType}" +
            $",{nameof(DataPackets)}.Length={DataPackets?.Length}";
    }
}
