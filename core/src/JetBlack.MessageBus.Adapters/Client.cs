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

using JetBlack.MessageBus.Common;
using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Adapters.Configuration;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Adapters
{
    public class Client : IClient
    {
        public static Client Create(ClientConfig config, IClientAuthenticator authenticator, bool autoConnect)
        {
            if (config.Server == null)
                throw new ArgumentException("The server must be specified");

            var byteEncoder = config.ByteEncoderType?.LoadType()?.Construct<IByteEncoder>() ?? new BinaryEncoder();

            return Create(
              Environment.ExpandEnvironmentVariables(config.Server),
              config.Port,
              byteEncoder,
              config.MonitorHeartbeat,
              config.IsSslEnabled,
              authenticator,
              autoConnect);
        }

        public static Client Create(
            string server,
            int port,
            IByteEncoder byteEncoder,
            bool monitorHeartbeat = false,
            bool isSslEnabled = false,
            IClientAuthenticator? authenticator = null,
            bool autoConnect = true)
        {
            var ipAddress = Dns.GetHostEntry(server).AddressList
               .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            var endpoint = new IPEndPoint(ipAddress, 9091);

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

            var client = new Client(stream, byteEncoder);

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

        public event EventHandler<DataReceivedEventArgs>? OnDataReceived;
        public event EventHandler<DataErrorEventArgs>? OnDataError;
        public event EventHandler<ForwardedSubscriptionEventArgs>? OnForwardedSubscription;
        public event EventHandler<AuthorizationRequestEventArgs>? OnAuthorizationRequest;
        public event EventHandler<ConnectionChangedEventArgs>? OnConnectionChanged;
        public event EventHandler<EventArgs>? OnHeartbeat;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Stream _stream;
        private readonly IByteEncoder _byteEncoder;
        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();

        internal Client(Stream stream, IByteEncoder byteEncoder)
        {
            _stream = stream;
            _byteEncoder = byteEncoder;
        }

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

        public void AddSubscription(string feed, string topic)
        {
            MakeSubscriptionRequest(feed, topic, true);
        }

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

        public void AddNotification(string feed)
        {
            MakeNotificationRequest(feed, true);
        }

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

        public void Send(Guid clientId, string feed, string topic, bool isImage, IReadOnlyList<DataPacket>? data)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            try
            {
                _writeQueue.Add(new UnicastData(clientId, feed, topic, isImage, Encode(data)));
            }
            catch (Exception error)
            {
                OnDataError?.Invoke(this, new DataErrorEventArgs(true, string.Empty, string.Empty, feed, topic, isImage, data, error));
            }
        }

        public void Publish(string feed, string topic, bool isImage, IReadOnlyList<DataPacket>? data)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            try
            {
                _writeQueue.Add(new MulticastData(feed, topic, isImage, Encode(data)));
            }
            catch (Exception error)
            {
                OnDataError?.Invoke(this, new DataErrorEventArgs(true, string.Empty, string.Empty, feed, topic, isImage, data, error));
            }
        }

        public void Authorize(Guid clientId, string feed, string topic, bool isAuthorizationRequired, HashSet<int>? entitlements)
        {
            if (feed == null)
                throw new ArgumentNullException(nameof(feed));
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            _writeQueue.Add(new AuthorizationResponse(clientId, feed, topic, isAuthorizationRequired, entitlements));
        }

        private void RaiseOnAuthorizationRequest(AuthorizationRequest message)
        {
            OnAuthorizationRequest?.Invoke(this, new AuthorizationRequestEventArgs(message.ClientId, message.Host, message.User, message.Feed, message.Topic));
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
                RaiseOnData(message.User, message.Host, message.Feed, message.Topic, message.Data, message.IsImage);
        }

        private void RaiseOnHeartbeat()
        {
            OnHeartbeat?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseOnData(ForwardedUnicastData message)
        {
            RaiseOnData(message.User, message.Host, message.Feed, message.Topic, message.Data, message.IsImage);
        }

        private BinaryDataPacket[]? Encode(IReadOnlyList<DataPacket>? data)
        {
            if (data == null)
                return null;

            var encoded = new BinaryDataPacket[data.Count];
            for (var i = 0; i < data.Count; ++i)
                encoded[i] = new BinaryDataPacket(data[i].Entitlements, _byteEncoder.Encode(data[i].Data));
            return encoded;
        }

        protected DataPacket[]? Decode(BinaryDataPacket[]? data)
        {
            if (data == null)
                return null;

            var decoded = new DataPacket[data.Length];
            for (int i = 0; i < data.Length; ++i)
                decoded[i] = new DataPacket(data[i].Entitlements, _byteEncoder.Decode(data[i].Data));

            return decoded;
        }

        private void RaiseOnData(string user, string host, string feed, string topic, BinaryDataPacket[]? data, bool isImage)
        {
            try
            {
                OnDataReceived?.Invoke(this, new DataReceivedEventArgs(user, host, feed, topic, Decode(data), isImage));
            }
            catch (Exception error)
            {
                OnDataError?.Invoke(this, new DataErrorEventArgs(false, user, host, feed, topic, isImage, data, error));
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _stream.Close();
        }
    }
}
