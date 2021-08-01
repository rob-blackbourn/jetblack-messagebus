#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The event arguments received data has been published to a subscribed feed and topic.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        internal DataReceivedEventArgs(string user, string host, string feed, string topic, DataPacket[]? dataPackets, bool isImage)
        {
            User = user;
            Host = host;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
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
        /// An indication of whether the data represents an image.
        /// </summary>
        public bool IsImage { get; }
        /// <summary>
        /// The data packets.
        /// </summary>
        public DataPacket[]? DataPackets { get; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(User)}={User}" +
            $"{nameof(Host)}={Host}" +
            $"{nameof(Feed)}={Feed}" +
            $"{nameof(Topic)}={Topic}" +
            $"{nameof(IsImage)}={IsImage}" +
            $",{nameof(DataPackets)}=\"{DataPackets?.Length ?? 0}\"";
    }
}
