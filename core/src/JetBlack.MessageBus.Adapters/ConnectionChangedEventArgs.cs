#nullable enable

using System;

namespace JetBlack.MessageBus.Adapters
{
    public class ConnectionChangedEventArgs : EventArgs
    {
        public ConnectionChangedEventArgs(ConnectionState state, Exception? error = null)
        {
            State = state;
            Error = error;
        }

        public ConnectionState State { get; }
        public Exception? Error { get; }
    }

    public enum ConnectionState
    {
        Connecting,
        Connected,
        Closed,
        Faulted
    }
}
