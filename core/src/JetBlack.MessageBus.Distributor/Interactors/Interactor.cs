#nullable enable

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using log4net;

using JetBlack.MessageBus.Messages;
using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class Interactor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Interactor));

        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();
        private readonly Stream _stream;
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly CancellationToken _token;
        private RoleManager? _roleManager;

        public static Interactor Create(
            TcpClient tcpClient,
            X509Certificate2? certificate,
            IAuthenticator authenticator,
            DistributorRole distributorRole,
            EventQueue<InteractorEventArgs> eventQueue,
            CancellationToken token)
        {
            var stream = (Stream)tcpClient.GetStream();
            if (certificate != null)
            {
                var sslStream = new SslStream(stream, false);
                sslStream.AuthenticateAsServer(certificate, clientCertificateRequired: false, checkCertificateRevocation: true);
                stream = sslStream;
            }

            var address = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
            var hostName = address.Equals(IPAddress.Loopback) ? Dns.GetHostName() : Dns.GetHostEntry(address).HostName;

            var identity = authenticator.Authenticate(stream);
            Log.Info($"Authenticated with {identity.AuthenticationType} as {identity.Name}");
            var roleManager = new RoleManager(distributorRole, hostName, identity.Name);

            var interactor = new Interactor(stream, hostName, identity.Name, roleManager, eventQueue, token);
            return interactor;
        }

        internal Interactor(
            Stream stream,
            string hostName,
            string userName,
            RoleManager roleManager,
            EventQueue<InteractorEventArgs> eventQueue,
            CancellationToken token)
        {
            _stream = stream;
            Id = Guid.NewGuid();
            Host = hostName;
            User = userName;
            _roleManager = roleManager;
            _token = token;
            _eventQueue = eventQueue;
        }

        public Guid Id { get; }
        public string Host { get; }
        public string User { get; }

        public bool HasRole(string feed, Role role)
        {
            return _roleManager != null && _roleManager.HasRole(feed, role);
        }

        public void Start()
        {
            Task.Run(() => QueueReceivedMessages(), _token);
            Task.Run(() => WriteQueuedMessages(), _token);
        }

        private void QueueReceivedMessages()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var message = ReceiveMessage();
                    _eventQueue.Enqueue(new InteractorMessageEventArgs(this, message));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception error)
                {
                    Log.Warn($"Failed to receive message for {this}");
                    _eventQueue.Enqueue(new InteractorErrorEventArgs(this, error));
                    break;
                }
            }

            Log.Debug($"Exited read loop for {this}");
        }

        private void WriteQueuedMessages()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var message = _writeQueue.Take(_token);
                    message.Write(new DataWriter(_stream));
                    _stream.Flush();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception error)
                {
                    _eventQueue.Enqueue(new InteractorErrorEventArgs(this, error));
                    break;
                }
            }

            Log.Debug($"Exited read loop for {this}");
        }

        public void SendMessage(Message message)
        {
            _writeQueue.Add(message, _token);
        }

        public Message ReceiveMessage()
        {
            return Message.Read(new DataReader(_stream));
        }

        public int CompareTo(Interactor? other)
        {
            return other == null ? 1 : Id.CompareTo(other.Id);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Interactor);
        }

        public bool Equals(Interactor? other)
        {
            return other != null && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}: {Host}";
        }

        public void Dispose()
        {
            _stream.Close();
        }
    }
}
