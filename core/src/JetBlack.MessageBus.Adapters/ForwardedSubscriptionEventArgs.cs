#nullable enable

using System;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The event arguments received when feed on which a notification has been placed has received a subscription request.
    /// </summary>
    public class ForwardedSubscriptionEventArgs : EventArgs
    {
        internal ForwardedSubscriptionEventArgs(string user, string host, Guid clientId, string feed, string topic, bool isAdd)
        {
            User = user;
            Host = host;
            ClientId = clientId;
            Feed = feed;
            Topic = topic;
            IsAdd = isAdd;
        }

        /// <summary>
        /// The name of the user that has made the subscription request.
        /// </summary>
        public string User { get; }
        /// <summary>
        /// The host from which the subscription request was made.
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// The identifier of the client that made the subscription request.
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
        /// If true the subscription has been add; otherwise it has been removed.
        /// </summary>
        public bool IsAdd { get; }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() => $"{nameof(User)}=\"{User}\",{nameof(Host)}=\"{Host}\",{nameof(ClientId)}={ClientId},{nameof(Feed)}=\"{Feed}\",{nameof(Topic)}=\"{Topic}\",IsAdd={IsAdd}";
    }
}
