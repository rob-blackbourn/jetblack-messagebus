using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public abstract class Message
    {
        protected Message(MessageType messageType)
        {
            MessageType = messageType;
        }

        public MessageType MessageType { get; }

        public static Message Read(DataReader reader)
        {
            var messageType = ReadHeader(reader);

            switch (messageType)
            {
                case MessageType.AuthorizationRequest:
                    return AuthorizationRequest.ReadBody(reader);
                case MessageType.AuthorizationResponse:
                    return AuthorizationResponse.ReadBody(reader);
                case MessageType.MulticastData:
                    return MulticastData.ReadBody(reader);
                case MessageType.UnicastData:
                    return UnicastData.ReadBody(reader);
                case MessageType.ForwardedMulticastData:
                    return ForwardedMulticastData.ReadBody(reader);
                case MessageType.ForwardedUnicastData:
                    return ForwardedUnicastData.ReadBody(reader);
                case MessageType.ForwardedSubscriptionRequest:
                    return ForwardedSubscriptionRequest.ReadBody(reader);
                case MessageType.NotificationRequest:
                    return NotificationRequest.ReadBody(reader);
                case MessageType.SubscriptionRequest:
                    return SubscriptionRequest.ReadBody(reader);
                default:
                    throw new InvalidDataException($"Unknown message type {messageType}");
            }
        }

        private static MessageType ReadHeader(DataReader reader)
        {
            var b = reader.ReadByte();
            return (MessageType)b;
        }

        public virtual DataWriter Write(DataWriter writer)
        {
            writer.Write((byte)MessageType);
            return writer;
        }

        public override string ToString() => $"{nameof(MessageType)}={MessageType}";
    }
}
