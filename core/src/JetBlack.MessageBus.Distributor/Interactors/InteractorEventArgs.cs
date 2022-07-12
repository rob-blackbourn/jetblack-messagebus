using System;

using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class InteractorEventArgs : EventArgs
    {
        public InteractorEventArgs(Interactor interactor)
        {
            Interactor = interactor;
        }

        public Interactor Interactor { get; }

        public override string ToString()
        {
            return $"Interactor={Interactor}";
        }
    }

    public class InteractorErrorEventArgs : InteractorEventArgs
    {
        public InteractorErrorEventArgs(Interactor interactor, Exception error)
            : base(interactor)
        {
            Error = error;
        }

        public Exception Error { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, Error={Error?.Message}";
        }
    }

    public class InteractorClosedEventArgs : InteractorEventArgs
    {
        public InteractorClosedEventArgs(Interactor interactor)
            : base(interactor)
        {
        }
    }

    public class InteractorConnectedEventArgs : InteractorEventArgs
    {
        public InteractorConnectedEventArgs(Interactor interactor)
            : base(interactor)
        {
        }
    }

    public class InteractorFaultedEventArgs : InteractorEventArgs
    {
        public InteractorFaultedEventArgs(Interactor interactor, Exception error)
            : base(interactor)
        {
            Error = error;
        }

        public Exception Error { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, Error={Error?.Message}";
        }
    }

    public class InteractorMessageEventArgs : InteractorEventArgs
    {
        public InteractorMessageEventArgs(Interactor interactor, Message message)
            : base(interactor)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
