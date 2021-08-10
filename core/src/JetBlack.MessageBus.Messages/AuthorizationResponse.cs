#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// An authorization response.
    /// </summary>
    public class AuthorizationResponse : Message, IEquatable<AuthorizationResponse>
    {
        /// <summary>
        /// Construct an authorization response.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="feed">The feed name.</param>
        /// <param name="topic">The topic name</param>
        /// <param name="isAuthorizationRequired">Indicates whether entitlements are required.</param>
        /// <param name="entitlements">The entitlements.</param>
        public AuthorizationResponse(Guid clientId, string feed, string topic, bool isAuthorizationRequired, ISet<int>? entitlements)
            : base(MessageType.AuthorizationResponse)
        {
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAuthorizationRequired = isAuthorizationRequired;
            Entitlements = entitlements;
        }

        /// <summary>
        /// The client id.
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
        /// If true then authorization is required, so entitlements should be used.
        /// </summary>
        public bool IsAuthorizationRequired { get; }
        /// <summary>
        /// The entitlements for an authorized feed/topic.
        /// </summary>
        public ISet<int>? Entitlements { get; }

        /// <summary>
        /// Read the message body.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The authorization response.</returns>
        public static AuthorizationResponse ReadBody(DataReader reader)
        {
            var clientId = reader.ReadGuid();
            var feed = reader.ReadString();
            var topic = reader.ReadString();
            var isAuthorizationRequired = reader.ReadBoolean();
            var entitlements = reader.ReadInt32Set();
            return new AuthorizationResponse(clientId, feed, topic, isAuthorizationRequired, entitlements);
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">Thje message to test.</param>
        /// <returns>True if the messages are equal.</returns>
        public bool Equals(AuthorizationResponse? other)
        {
            return other != null &&
                ClientId == other.ClientId &&
                Feed == other.Feed &&
                Topic == other.Topic &&
                IsAuthorizationRequired == other.IsAuthorizationRequired &&
                Entitlements.SequenceEqual(other.Entitlements);
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as AuthorizationResponse);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
                ClientId.GetHashCode() ^
                Feed.GetHashCode() ^
                Topic.GetHashCode() ^
                (Entitlements?.GetHashCode() ?? 0);
        }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(ClientId)}={ClientId}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"" +
            $",{nameof(IsAuthorizationRequired)}={IsAuthorizationRequired}" +
            $",{nameof(Entitlements)}.Count={Entitlements?.Count ?? 0}";
    }
}
