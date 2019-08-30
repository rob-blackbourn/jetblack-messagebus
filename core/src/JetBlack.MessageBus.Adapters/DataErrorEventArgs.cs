#nullable enable

using System;

namespace JetBlack.MessageBus.Adapters
{
    public class DataErrorEventArgs : EventArgs
    {
        public DataErrorEventArgs(bool isSending, string user, string host, string feed, string topic, bool isImage, object? data, Exception error)
        {
            IsSending = isSending;
            User = user;
            Host = host;
            Feed = feed;
            Topic = topic;
            IsImage = isImage;
            Data = data;
            Error = error;
        }

        public bool IsSending { get; }
        public string User { get; }
        public string Host { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsImage { get; }
        public object? Data { get; }
        public Exception Error { get; }
    }
}
