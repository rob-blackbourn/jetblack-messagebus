using System.IO;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    /// <summary>
    /// The base message.
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Construct a message.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        protected Message(MessageType messageType)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// The message type.
        /// </summary>
        public MessageType MessageType { get; }

        /// <summary>
        /// Read a message.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The message.</returns>
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

        /// <summary>
        /// Read a message header.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The type of the message.</returns>
        private static MessageType ReadHeader(DataReader reader)
        {
            var b = reader.ReadByte();
            return (MessageType)b;
        }

        /// <summary>
        /// Write a message.
        /// </summary>
        /// <param name="writer">The message writer.</param>
        /// <returns>The message writer.</returns>
        public virtual DataWriter Write(DataWriter writer)
        {
            writer.Write((byte)MessageType);
            return writer;
        }

        /// <summary>
        /// Converts the value of the current object to it's equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString() => $"{nameof(MessageType)}={MessageType}";
    }
}
