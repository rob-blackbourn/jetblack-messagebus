#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Adapters
{
    /// <summary>
    /// The message bus client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <param name="server">The server host name.</param>
        /// <param name="port">The server port</param>
        /// <param name="monitorHeartbeat">If true send heartbeats.</param>
        /// <param name="isSslEnabled">If true use SSL.</param>
        /// <param name="authenticator">The client authenticator.</param>
        /// <param name="autoConnect">If true automatically connect to the message bus.</param>
        /// <returns></returns>
        public static Client Create(
            string server,
            int port,
            bool monitorHeartbeat = false,
            bool isSslEnabled = false,
            ClientAuthenticator? authenticator = null,
            bool autoConnect = true)
        {
            var ipAddress = Dns.GetHostEntry(server).AddressList
               .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            var endpoint = new IPEndPoint(ipAddress, port);

            var tcpClient = new TcpClient();
            tcpClient.Connect(endpoint.Address, endpoint.Port);

            var stream = (Stream)tcpClient.GetStream();
            if (isSslEnabled)
            {
                var sslStream = new SslStream(
                    stream,
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null);
                sslStream.AuthenticateAsClient(server);
                stream = sslStream;
            }

            if (authenticator != null)
                authenticator.Authenticate(stream);

            var client = new Client(stream);

            if (autoConnect)
                client.Start();

            if (monitorHeartbeat)
                client.AddSubscription("__admin__", "heartbeat");

            return client;
        }

        private static bool ValidateServerCertificate(
          object sender,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }

        /// <summary>
        /// Raised when data has received from a subscribed feed and topic.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs>? OnDataReceived;
        /// <summary>
        /// Raised when a requested notification is received.
        /// </summary>
        public event EventHandler<ForwardedSubscriptionEventArgs>? OnForwardedSubscription;
        /// <summary>
        /// Raised when authorisation is requested.
        /// </summary>
        public event EventHandler<AuthorizationRequestEventArgs>? OnAuthorizationRequest;
        /// <summary>
        /// Raised when the connection state has changed.
        /// </summary>
        public event EventHandler<ConnectionChangedEventArgs>? OnConnectionChanged;
        /// <summary>
        /// Raised when a heartbeat has been received.
        /// </summary>
        public event EventHandler<EventArgs>? OnHeartbeat;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Stream _stream;
        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();

        internal Client(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Start processing message bus data.
        /// </summary>
        public void Start()
        {
            Task.Run(() => Read(), _cancellationTokenSource.Token);
            Task.Run(() => Write(), _cancellationTokenSource.Token);
        }

        private void Read()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = Message.Read(new DataReader(_stream));

                    switch (message.MessageType)
                    {
                        case MessageType.AuthorizationRequest:
                            RaiseOnAuthorizationRequest((AuthorizationRequest)message);
                            break;
                        case MessageType.ForwardedMulticastData:
                            RaiseOnDataOrHeartbeat((ForwardedMulticastData)message);
                            break;
                        case MessageType.ForwardedUnicastData:
                            RaiseOnData((ForwardedUnicastData)message);
                            break;
                        case MessageType.ForwardedSubscriptionRequest:
                            RaiseOnForwardedSubscriptionRequest((ForwardedSubscriptionRequest)message);
                            break;
                        default:
                            throw new ArgumentException("invalid message type");
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (EndOfStreamException)
                {
                    RaiseConnectionStateChanged(ConnectionState.Closed);
                    return;
                }
                catch (Exception error)
                {
                    RaiseConnectionStateChanged(ConnectionState.Faulted, error);
                    return;
                }
            }
        }

        private void Write()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = _writeQueue.Take(_cancellationTokenSource.Token);
                    message.Write(new DataWriter(_stream));
                    _stream.Flush();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (EndOfStreamException)
                {
                    RaiseConnectionStateChanged(ConnectionState.Closed);
                    return;
                }
                catch (Exception error)
                {
                    RaiseConnectionStateChanged(ConnectionState.Faulted, error);
                    return;
                }
            }
        }

        private void RaiseConnectionStateChanged(ConnectionState state, Exception? error = null)
        {
            OnConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(state, error));
        }

        /// <summary>
        /// Add a subscription to a feed and topic.
        /// </summary>
        /// <param name="feed">The feed name.</param>
        /// <param name="topic">The topic name</param>
        public void AddSubscription(string feed, string topic)
        {
            MakeSubscriptionRequest(feed, topic, true);
        }

        /// <summary>
        /// Remove a subscription to a feed and topic.
        /// </summary>
        /// <param name="feed">The feed name.</param>
        /// <param name="topic">The topic name</param>
        public void RemoveSubscription(string feed, string topic)
        {
            MakeSubscriptionRequest(feed, topic, false);
        }

        private void MakeSubscriptionRequest(string feed, string topic, bool isAdd)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new SubscriptionRequest(feed, topic, isAdd));
        }

        /// <summary>
        /// Request notifications of subscriptions to a feed.
        /// </summary>
        /// <param name="feed">The feed name.</param>
        public void AddNotification(string feed)
        {
            MakeNotificationRequest(feed, true);
        }

        /// <summary>
        /// Remove notification of subscriptions to a feed.
        /// </summary>
        /// <param name="feed">The feed name.</param>
        public void RemoveNotification(string feed)
        {
            MakeNotificationRequest(feed, false);
        }

        private void MakeNotificationRequest(string feed, bool isAdd)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));

            _writeQueue.Add(new NotificationRequest(feed, isAdd));
        }

        /// <summary>
        /// Send a message to a specific client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="feed">The feed name.</param>
        /// <param name="topic">The topic name</param>
        /// <param name="isImage">A boolean indicating if the message was an image.</param>
        /// <param name="dataPackets">The data packets to send.</param>
        public void Send(Guid clientId, string feed, string topic, bool isImage, DataPacket[]? dataPackets)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new UnicastData(clientId, feed, topic, isImage, dataPackets));
        }

        /// <summary>
        /// Publish a message to the subscribers of a feed and topic.
        /// </summary>
        /// <param name="feed">The feed name.</param>
        /// <param name="topic">The topic name</param>
        /// <param name="isImage">A boolean indicating if the message was an image.</param>
        /// <param name="dataPackets">The data packets to send.</param>
        public void Publish(string feed, string topic, bool isImage, DataPacket[]? dataPackets)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new MulticastData(feed, topic, isImage, dataPackets));
        }

        /// <summary>
        /// Authorise a client.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="feed">The feed name.</param>
        /// <param name="topic">The topic name</param>
        /// <param name="isAuthorizationRequired">If true authorisation is required.</param>
        /// <param name="entitlements">The set of entitlements granted to the client.</param>
        public void Authorize(Guid clientId, string feed, string topic, bool isAuthorizationRequired, ISet<int>? entitlements)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new AuthorizationResponse(clientId, feed, topic, isAuthorizationRequired, entitlements));
        }

        private void RaiseOnAuthorizationRequest(AuthorizationRequest message)
        {
            OnAuthorizationRequest?.Invoke(
                this,
                new AuthorizationRequestEventArgs(
                    message.ClientId,
                    message.Host,
                    message.User,
                    message.Feed,
                    message.Topic));
        }

        private void RaiseOnForwardedSubscriptionRequest(ForwardedSubscriptionRequest message)
        {
            OnForwardedSubscription?.Invoke(
                this,
                new ForwardedSubscriptionEventArgs(
                    message.User,
                    message.Host,
                    message.ClientId,
                    message.Feed,
                    message.Topic,
                    message.IsAdd));
        }

        private void RaiseOnDataOrHeartbeat(ForwardedMulticastData message)
        {
            if (message.Feed == "__admin__" && message.Topic == "heartbeat")
                RaiseOnHeartbeat();
            else
                RaiseOnData(message.User, message.Host, message.Feed, message.Topic, message.DataPackets, message.IsImage);
        }

        private void RaiseOnHeartbeat()
        {
            OnHeartbeat?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseOnData(ForwardedUnicastData message)
        {
            RaiseOnData(message.User, message.Host, message.Feed, message.Topic, message.DataPackets, message.IsImage);
        }

        private void RaiseOnData(string user, string host, string feed, string topic, DataPacket[]? dataPackets, bool isImage)
        {
            OnDataReceived?.Invoke(this, new DataReceivedEventArgs(user, host, feed, topic, dataPackets, isImage));
        }

        /// <summary>
        /// Dispose of the client, closing the connection.
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _stream.Close();
        }
    }
}
