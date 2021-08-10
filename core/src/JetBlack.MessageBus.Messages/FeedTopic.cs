using System;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// A feed and topic name.
    /// </summary>
    public struct FeedTopic : IEquatable<FeedTopic>
    {
        /// <summary>
        /// The name of the feed.
        /// </summary>
        public readonly string Feed;
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public readonly string Topic;

        /// <summary>
        /// Construct a feed topic.
        /// </summary>
        /// <param name="feed">The name of the feed.</param>
        /// <param name="topic">The name of the topic.</param>
        public FeedTopic(string feed, string topic)
        {
            Feed = feed;
            Topic = topic;
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="other">The value to test.</param>
        /// <returns>True if the feed topics are equal.</returns>
        public bool Equals(FeedTopic other)
        {
            return Feed == other.Feed && Topic == other.Topic;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return (Feed ?? string.Empty).GetHashCode() ^ (Topic ?? string.Empty).GetHashCode();
        }

        /// <summary>
        /// Test for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is FeedTopic && Equals((FeedTopic)obj);
        }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() => $"{nameof(Feed)}=\"{Feed}\",{nameof(Topic)}=\"{Topic}\"";
    }
}
