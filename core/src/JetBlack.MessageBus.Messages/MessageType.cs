namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// The message type.
    /// </summary>
    public enum MessageType : byte
    {
        MulticastData = 1,
        UnicastData = 2,
        ForwardedSubscriptionRequest = 3,
        NotificationRequest = 4,
        SubscriptionRequest = 5,
        AuthorizationRequest = 6,
        AuthorizationResponse = 7,
        ForwardedMulticastData = 8,
        ForwardedUnicastData = 9
    }
}
