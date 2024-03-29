using System;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The event arguments received data has been published to a subscribed feed and topic.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        internal DataReceivedEventArgs(string user, string host, string feed, string topic, DataPacket[]? dataPackets, string contentType)
        {
            User = user;
            Host = host;
            Feed = feed;
            Topic = topic;
            ContentType = contentType;
            DataPackets = dataPackets;
        }

        /// <summary>
        /// The name of the user that published the data.
        /// </summary>
        public string User { get; }
        /// <summary>
        /// The name of the host from which the data was published.
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
        /// The content type of the message.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// The data packets.
        /// </summary>
        public DataPacket[]? DataPackets { get; }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{nameof(User)}={User}" +
            $"{nameof(Host)}={Host}" +
            $"{nameof(Feed)}={Feed}" +
            $"{nameof(Topic)}={Topic}" +
            $"{nameof(ContentType)}={ContentType}" +
            $",{nameof(DataPackets)}=\"{DataPackets?.Length ?? 0}\"";
    }
}
