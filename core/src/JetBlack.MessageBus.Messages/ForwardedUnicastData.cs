#nullable enable

using System;
using System.Linq;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// When a client sends a message to another client it gets sent to the broker as a unicast data
    /// message, then forwarded to the destination client as a forwarded unicast data message.
    /// </summary>
    public class ForwardedUnicastData : Message, IEquatable<ForwardedUnicastData>
    {
        /// <summary>
        /// Construct the message.
        /// </summary>
        /// <param name="user">The user that sent the data.</param>
        /// <param name="host">The host from which the data was sent.</param>
        /// <param name="clientId">The id of the client that sent the message.</param>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="topic">The name of the topic.</param>
        /// <param name="isImage">If true the data is considered to be an image.</param>
        /// <param name="dataPackets">The data packets.</param>
        public ForwardedUnicastData(string user, string host, Guid clientId, string feed, string topic, bool isImage, DataPacket[]? dataPackets)
            : base(MessageType.ForwardedUnicastData)
        {
            User = user;
            Host = host;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            DataPackets = dataPackets;
        }

        /// <summary>
        /// The user that sent hte data.
        /// </summary>
        public string User { get; }
        /// <summary>
        /// The host from whence the data was sent.
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// The id of the client that sent the data.
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
        /// If true the data is considered to be an image.
        /// </summary>
        public bool IsImage { get; }
        /// <summary>
        /// The data packets.
        /// </summary>
        public DataPacket[]? DataPackets { get; }

        /// <summary>
        /// Read the message body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The message.</returns>
        public static ForwardedUnicastData ReadBody(DataReader reader)
        {
            var user = reader.ReadString();
            var host = reader.ReadString();
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isImage = reader.ReadBoolean();
            var dataPackets = reader.ReadBinaryDataPacketArray();
            return new ForwardedUnicastData(user, host, clientId, feed, topic, isImage, dataPackets);
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
            writer.Write(IsImage);
            writer.Write(DataPackets);
            return writer;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The message to test against.</param>
        /// <returns>True if the messages are equal.</returns>
        public bool Equals(ForwardedUnicastData? other)
        {
            return other != null &&
                User == other.User &&
                Host == other.Host &&
                ClientId == other.ClientId &&
                Feed == other.Feed &&
                Topic == other.Topic &&
              IsImage == other.IsImage &&
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
            return Equals(obj as ForwardedUnicastData);
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
              IsImage.GetHashCode() ^
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
            $",{nameof(ClientId)}={ClientId}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(IsImage)}={IsImage}" +
            $",{nameof(DataPackets)}.Length={DataPackets?.Length}";
    }
}
