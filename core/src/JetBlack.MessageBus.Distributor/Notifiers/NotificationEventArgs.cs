#nullable enable

using JetBlack.MessageBus.Distributor.Interactors;

namespace JetBlack.MessageBus.Distributor.Notifiers
{
    public class NotificationEventArgs : InteractorEventArgs
    {
        public NotificationEventArgs(Interactor interactor, string feed)
            : base(interactor)
        {
            Feed = feed;
        }

        public string Feed { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, Feed={Feed}";
        }
    }
}
