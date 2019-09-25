#nullable enable

namespace JetBlack.MessageBus.Adapters
{
    public class NullClientAuthenticator : ClientAuthenticator
    {
        protected override string ToConnectionString() => string.Empty;
    }
}