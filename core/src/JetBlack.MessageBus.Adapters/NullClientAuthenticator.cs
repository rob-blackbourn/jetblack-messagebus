namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The client authenticator to use when authentication is not required.
    /// </summary>
    public class NullClientAuthenticator : ClientAuthenticator
    {
        /// <inheritdoc />
        protected override string ToConnectionString() => string.Empty;
    }
}