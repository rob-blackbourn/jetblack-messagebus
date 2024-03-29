using System;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The events args provided when the state of the connection changes.
    /// </summary>
    public class ConnectionChangedEventArgs : EventArgs
    {
        internal ConnectionChangedEventArgs(ConnectionState state, Exception? error = null)
        {
            State = state;
            Error = error;
        }

        /// <summary>
        /// The connection state.
        /// </summary>
        public ConnectionState State { get; }
        /// <summary>
        /// The error for connection failures.
        /// </summary>
        public Exception? Error { get; }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() =>
            $"{nameof(State)}={State}" +
            $",{nameof(Error)}=\"{Error?.Message}\"";
    }

    /// <summary>
    /// The connection state.
    /// </summary>
    public enum ConnectionState
    {
        Connecting,
        Connected,
        Closed,
        Faulted
    }
}
