using System;

namespace JetBlack.MessageBus.Messages
{
    public struct FeedTopic : IEquatable<FeedTopic>
    {
        public readonly string Feed;
        public readonly string Topic;

        public FeedTopic(string feed, string topic)
        {
            Feed = feed;
            Topic = topic;
        }

        public bool Equals(FeedTopic other)
        {
            return Feed == other.Feed && Topic == other.Topic;
        }

        public override int GetHashCode()
        {
            return (Feed ?? string.Empty).GetHashCode() ^ (Topic ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is FeedTopic && Equals((FeedTopic)obj);
        }

        public override string ToString()
        {
            return $"Feed={Feed}, Topic={Topic}";
        }
    }
}
