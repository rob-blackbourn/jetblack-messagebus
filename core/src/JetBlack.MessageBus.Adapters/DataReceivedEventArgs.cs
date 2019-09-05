#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(string user, string host, string feed, string topic, DataPacket[]? dataPackets, bool isImage)
        {
            User = user;
            Host = host;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            DataPackets = dataPackets;
        }

        public string User { get; }
        public string Host { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public DataPacket[]? DataPackets { get; }
    }
}
