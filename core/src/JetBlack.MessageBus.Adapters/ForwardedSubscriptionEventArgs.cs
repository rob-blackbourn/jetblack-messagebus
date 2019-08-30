#nullable enable

using System;

namespace JetBlack.MessageBus.Adapters
{
    public class ForwardedSubscriptionEventArgs : EventArgs
    {
        public ForwardedSubscriptionEventArgs(string user, string host, Guid clientId, string feed, string topic, bool isAdd)
        {
            User = user;
            Host = host;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        public string User { get; }
        public string Host { get; }
        public Guid ClientId { get; }
        public string Feed { get; }
        public string Topic { get; }
        public bool IsAdd { get; }

        public override string ToString() => $"{nameof(User)}=\"{User}\",{nameof(Host)}=\"{Host}\",{nameof(ClientId)}={ClientId},{nameof(Feed)}=\"{Feed}\",{nameof(Topic)}=\"{Topic}\",IsAdd={IsAdd}";
    }
}
