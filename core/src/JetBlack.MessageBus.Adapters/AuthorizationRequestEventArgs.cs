using System;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The event args for an authorization request.
    /// </summary>
    public class AuthorizationRequestEventArgs : EventArgs
    {
        internal AuthorizationRequestEventArgs(Guid clientId, string host, string user, string feed, string topic)
        {
            ClientId = clientId;
            Host = host;
            User = user;
            Feed = feed;
            Topic = topic;
        }

        /// <summary>
        /// The client identifier.
        /// </summary>
        public Guid ClientId { get; private set; }
        /// <summary>
        /// The name of the host.
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string User { get; private set; }
        /// <summary>
        /// The name of the feed.
        /// </summary>
        public string Feed { get; private set; }
        /// <summary>
        /// The name of the topic.
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{nameof(ClientId)}={ClientId}" +
            $",{nameof(Host)}=\"{Host}\"" +
            $",{nameof(User)}=\"{User}\"" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"";
    }
}
