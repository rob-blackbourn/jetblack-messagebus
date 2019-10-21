#nullable enable

using System;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages
{
    public class NotificationRequest : Message, IEquatable<NotificationRequest>
    {
        public NotificationRequest(string feed, bool isAdd)
            : base(MessageType.NotificationRequest)
        {
            Feed = feed;
            IsAdd = isAdd;
        }

        public string Feed { get; }
        public bool IsAdd { get; }

        public static NotificationRequest ReadBody(DataReader reader)
        {
            var feed = reader.ReadString();
            var isAdd = reader.ReadBoolean();
            return new NotificationRequest(feed, isAdd);
        }

        public override DataWriter Write(DataWriter writer)
        {
            base.Write(writer);
            writer.Write(Feed);
            writer.Write(IsAdd);
            return writer;
        }

        public bool Equals(NotificationRequest? other)
        {
            return other != null &&
              MessageType == other.MessageType &&
              Feed == other.Feed &&
              IsAdd == other.IsAdd;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as NotificationRequest);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() ^
              (Feed ?? "").GetHashCode() ^
              IsAdd.GetHashCode();
        }

        public override string ToString() =>
            $"{base.ToString()}" +
            $",{nameof(Feed)}=\"{Feed}\"" +
            $",{nameof(IsAdd)}={IsAdd}";
    }
}
