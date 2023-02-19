using System;
using System.Linq;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// A forwarded multicast data message. When a client publishes, multi data is sent to the distributor, it
    /// gets forwarded to the subscribing clients with this message.
    /// </summary>
    public class ForwardedMulticastData : Message, IEquatable<ForwardedMulticastData>
    {
        /// <summary>
        /// Construct a forwarded multicast data message.
        /// </summary>
        /// <param name="user">The user that sent the message.</param>
        /// <param name="host">The host of the sender.</param>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="topic">The name of the topic.</param>
        /// <param name="contentType">If true the data is considered an image.</param>
        /// <param name="dataPackets">The data packets.</param>
        public ForwardedMulticastData(string user, string host, string feed, string topic, string contentType, DataPacket[]? dataPackets)
            : base(MessageType.ForwardedMulticastData)
        {
            User = user;
            Host = host;
            Feed = feed;
            Topic = topic;
            ContentType = contentType;
            DataPackets = dataPackets;
        }

        /// <summary>
        /// The user that sent the message.
        /// </summary>
        public string User { get; }
        /// <summary>
        /// The host of the sender.
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// The name of the feed.
        /// </summary>
        public string Feed { get; }
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string Topic { get; }
        /// <summary>
        /// If true the data is considered an image.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// The data packets.
        /// </summary>
        public DataPacket[]? DataPackets { get; }

        /// <summary>
        /// Read the body of the message.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The message.</returns>
        public static ForwardedMulticastData ReadBody(DataReader reader)
        {
            var user = reader.ReadString();
            var host = reader.ReadString();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var contentType = reader.ReadString();
            var dataPackets = reader.ReadBinaryDataPacketArray();
            return new ForwardedMulticastData(user, host, feed, topic, contentType, dataPackets);
        }

        /// <inheritdoc />
        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(User);
            writer.Write(Host);
            writer.Write(Feed);
            writer.Write(Topic);
            writer.Write(ContentType);
            writer.Write(DataPackets);
            return writer;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The message to test.</param>
        /// <returns>True if the messages are equal.</returns>
        public bool Equals(ForwardedMulticastData? other)
        {
            return other != null &&
                User == other.User &&
                Host == other.Host &&
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
        public override bool Equals(object obj)
        {
            return Equals(obj as ForwardedMulticastData);
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
            $",{nameof(User)}=\"{User}\"" +
            $",{nameof(Host)}=\"{Host}\"" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(ContentType)}=\"{ContentType}\"" +
            $",{nameof(DataPackets)}.Length={DataPackets?.Length}";
    }
}
