#nullable enable

using System;
using System.Collections.Generic;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Adapters
{
    public interface IClient : IDisposable
    {
        event EventHandler<DataReceivedEventArgs>? OnDataReceived;
        event EventHandler<DataErrorEventArgs>? OnDataError;
        event EventHandler<ForwardedSubscriptionEventArgs>? OnForwardedSubscription;
        event EventHandler<AuthorizationRequestEventArgs>? OnAuthorizationRequest;
        event EventHandler<ConnectionChangedEventArgs>? OnConnectionChanged;
        event EventHandler<EventArgs>? OnHeartbeat;

        void AddSubscription(string feed, string topic);
        void RemoveSubscription(string feed, string topic);
        void AddNotification(string feed);
        void RemoveNotification(string feed);
        void Send(Guid clientId, string feed, string topic, bool isImage, IReadOnlyList<DataPacket>? data);
        void Publish(string feed, string topic, bool isImage, IReadOnlyList<DataPacket>? data);
        void Authorize(Guid clientId, string feed, string topic, bool isAuthorizationRequired, Guid[] entitlements);
    }
}
