using System;

namespace JetBlack.MessageBus.Adapters
{
    public class AuthorizationRequestEventArgs : EventArgs
    {
        public AuthorizationRequestEventArgs(Guid clientId, string host, string user, string? forwardedFor, string? impersonating, string feed, string topic)
        {
            ClientId = clientId;
            Host = host;
            User = user;
            ForwardedFor = forwardedFor;
            Impersonating = impersonating;
            Feed = feed;
            Topic = topic;
        }

        public Guid ClientId { get; private set; }
        public string Host { get; private set; }
        public string User { get; private set; }
        public string? ForwardedFor { get; private set; }
        public string? Impersonating { get; private set; }
        public string Feed { get; private set; }
        public string Topic { get; private set; }

        public override string ToString() =>
            $"{nameof(ClientId)}={ClientId}" +
            $",{nameof(Host)}=\"{Host}\"" +
            $",{nameof(User)}=\"{User}\"" +
            $",{nameof(ForwardedFor)}=\"{ForwardedFor}\"" +
            $",{nameof(Impersonating)}=\"{Impersonating}\"" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(Topic)}=\"{Topic}\"";
    }
}
