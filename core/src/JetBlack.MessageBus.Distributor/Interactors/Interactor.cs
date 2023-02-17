using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using JetBlack.MessageBus.Messages;
using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Distributor.Utilities;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class Interactor
    {
        private readonly ILogger<Interactor> _logger;
        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();
        private readonly IDictionary<string, IDictionary<string, AuthorizationResponse>> _authorizations = new Dictionary<string, IDictionary<string, AuthorizationResponse>>();
        private readonly CancellationTokenSource _tokenSource;
        private readonly Stream _stream;
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly RoleManager _roleManager;
        private readonly Task _readTask, _writeTask;

        public static Interactor Create(
            TcpClient tcpClient,
            X509Certificate2? certificate,
            IAuthenticator authenticator,
            DistributorRole distributorRole,
            EventQueue<InteractorEventArgs> eventQueue,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            var logger = loggerFactory.CreateLogger<Interactor>();

            var stream = GetStream(tcpClient, certificate, logger);

            var address = (tcpClient.Client.RemoteEndPoint as IPEndPoint)?.Address ?? IPAddress.Any;
            var hostName = address.Equals(IPAddress.Loopback) ? Dns.GetHostName() : Dns.GetHostEntry(address).HostName;

            var authenticationResponse = authenticator.Authenticate(stream);
            logger.LogInformation("Authenticated with {Type} as {Name}", authenticationResponse.Method, authenticationResponse.User);
            var roleManager = new RoleManager(
                distributorRole,
                hostName,
                authenticationResponse.User,
                authenticationResponse.Impersonating,
                authenticationResponse.ForwardedFor);

            return new Interactor(
                stream,
                authenticationResponse.Application ?? "unspecified",
                roleManager,
                eventQueue,
                logger,
                token);
        }

        private static Stream GetStream(TcpClient tcpClient, X509Certificate2? certificate, ILogger<Interactor> logger)
        {
            var stream = tcpClient.GetStream();
            if (certificate == null)
                return stream;

            try
            {
                var sslStream = new SslStream(stream, false);
                sslStream.AuthenticateAsServer(certificate, clientCertificateRequired: false, checkCertificateRevocation: true);
                return sslStream;
            }
            catch
            {
                throw new SslException((tcpClient.Client.RemoteEndPoint as IPEndPoint)?.Address ?? IPAddress.None);
            }
        }
        public static Interactor CreateSspi(
            TcpClient tcpClient,
            IAuthenticator authenticator,
            DistributorRole distributorRole,
            EventQueue<InteractorEventArgs> eventQueue,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            var logger = loggerFactory.CreateLogger<Interactor>();

            var stream = new NegotiateStream(tcpClient.GetStream(), false);
            stream.AuthenticateAsServer();

            var authenticationResponse = authenticator.Authenticate(stream);

            var address = (tcpClient.Client.RemoteEndPoint as IPEndPoint)?.Address ?? IPAddress.Any;
            var hostName = address.Equals(IPAddress.Loopback) ? Dns.GetHostName() : Dns.GetHostEntry(address).HostName;

            logger.LogInformation(
                "Authenticated with {Type} as {Name}",
                stream.RemoteIdentity.AuthenticationType,
                stream.RemoteIdentity.Name);

            var roleManager = new RoleManager(
                distributorRole,
                hostName,
                stream.RemoteIdentity.Name ?? "nobody",
                authenticationResponse.Impersonating,
                authenticationResponse.ForwardedFor);

            return new Interactor(
                stream,
                "sspi",
                roleManager,
                eventQueue,
                logger,
                token);
        }

        internal Interactor(
            Stream stream,
            string application,
            RoleManager roleManager,
            EventQueue<InteractorEventArgs> eventQueue,
            ILogger<Interactor> logger,
            CancellationToken token)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            _logger = logger;
            _stream = stream;
            Id = Guid.NewGuid();
            Application = application;
            _roleManager = roleManager;
            _eventQueue = eventQueue;
            _readTask = new Task(QueueReceivedMessages, _tokenSource.Token);
            _writeTask = new Task(WriteQueuedMessages, _tokenSource.Token);
            Metrics = new InteractorMetrics(User, Host, Id, Application);
        }

        public Guid Id { get; }
        public string Application { get; }
        public string Host => _roleManager.Host;
        public string User => _roleManager.User;
        public string? Impersonating => _roleManager.Impersonating;
        public string? ForwardedFor => _roleManager.ForwardedFor;
        public InteractorMetrics Metrics { get; }

        public bool HasRole(string feed, Role role)
        {
            return _roleManager.HasRole(feed, role);
        }

        public string UserForFeed(string feed) => _roleManager.UserForFeed(feed);
        public string HostForFeed(string feed) => _roleManager.HostForFeed(feed);
        public bool IsAuthorizationRequired(string feed) => _roleManager.IsAuthorizationRequired(feed);

        public void Start()
        {
            _readTask.Start();
            _writeTask.Start();
        }

        public void SendMessage(Message message)
        {
            try
            {
                _writeQueue.Add(message, _tokenSource.Token);
                Metrics.WriteQueueLength.Inc();
            }
            catch (OperationCanceledException)
            {
                // We may get post cancellation writes if the event queue still
                // contains messages for this interactor.
            }
        }

        public void SetAuthorization(string feed, string topic, AuthorizationResponse authorization)
        {
            if (!_authorizations.TryGetValue(feed, out var topicAuthorizations))
                _authorizations.Add(feed, topicAuthorizations = new Dictionary<string, AuthorizationResponse>());

            topicAuthorizations[topic] = authorization;
        }

        public bool TryGetAuthorization(string feed, string topic, out AuthorizationResponse? authorization)
        {
            if (_authorizations.TryGetValue(feed, out var topicAuthorizations))
                return topicAuthorizations.TryGetValue(topic, out authorization);

            authorization = null;
            return false;
        }

        public bool HasAuthorization(string feed, string topic)
        {
            return _authorizations.TryGetValue(feed, out var topicAuthorizations) && topicAuthorizations.ContainsKey(feed);
        }

        private void QueueReceivedMessages()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = Message.Read(new DataReader(_stream));
                    Metrics.ReadsReceived.Inc();
                    _eventQueue.Enqueue(new InteractorMessageEventArgs(this, message));
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

            _logger.LogDebug("Exited the read loop for {Interactor}.", this);

            if (!_tokenSource.IsCancellationRequested)
                _tokenSource.Cancel();
        }

        private void WriteQueuedMessages()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                try
                {
                    var message = _writeQueue.Take(_tokenSource.Token);
                    Metrics.WriteQueueLength.Dec();
                    message.Write(new DataWriter(_stream));
                    _stream.Flush();
                    Metrics.WritesSent.Inc();
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

            _logger.LogDebug("Exited the write loop for {Interactor}.", this);

            Metrics.WriteQueueLength.Set(0);

            if (!_tokenSource.IsCancellationRequested)
                _tokenSource.Cancel();
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

        public override string ToString() => $"{Id}: {User}({Impersonating}) {Host}({ForwardedFor})";

        public void Dispose()
        {
            if (!_tokenSource.IsCancellationRequested)
                _tokenSource.Cancel();

            if (!_readTask.IsCanceled)
                _readTask.Wait();
            if (!_writeTask.IsCanceled)
                _writeTask.Wait();

            _stream.Close();

            _tokenSource.Dispose();
        }
    }
}
