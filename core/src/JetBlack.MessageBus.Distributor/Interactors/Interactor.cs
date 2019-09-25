#nullable enable

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

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class Interactor
    {
        private readonly ILogger<Interactor> _logger;
        private readonly BlockingCollection<Message> _writeQueue = new BlockingCollection<Message>();
        private readonly IDictionary<string, IDictionary<string, AuthorizationResponse>> _authorizations = new Dictionary<string, IDictionary<string, AuthorizationResponse>>();
        private readonly Stream _stream;
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly CancellationToken _token;
        private readonly RoleManager _roleManager;

        public static Interactor Create(
            TcpClient tcpClient,
            X509Certificate2? certificate,
            IAuthenticator authenticator,
            DistributorRole distributorRole,
            EventQueue<InteractorEventArgs> eventQueue,
            ILoggerFactory loggerFactory,
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

            var authenticationResponse = authenticator.Authenticate(stream);
            var logger = loggerFactory.CreateLogger<Interactor>();
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

        internal Interactor(
            Stream stream,
            string application,
            RoleManager roleManager,
            EventQueue<InteractorEventArgs> eventQueue,
            ILogger<Interactor> logger,
            CancellationToken token)
        {
            _logger = logger;
            _stream = stream;
            Id = Guid.NewGuid();
            Application = application;
            _roleManager = roleManager;
            _token = token;
            _eventQueue = eventQueue;
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
            Task.Run(() => QueueReceivedMessages(), _token);
            Task.Run(() => WriteQueuedMessages(), _token);
        }

        public void SendMessage(Message message)
        {
            Metrics.WriteQueueLength.Inc();
            _writeQueue.Add(message, _token);
        }

        private Message ReceiveMessage()
        {
            return Message.Read(new DataReader(_stream));
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
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var message = ReceiveMessage();
                    Metrics.ReadsReceived.Inc();
                    _eventQueue.Enqueue(new InteractorMessageEventArgs(this, message));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception error)
                {
                    _logger.LogWarning(error, "Failed to receive message for {Interactor}", this);
                    _eventQueue.Enqueue(new InteractorErrorEventArgs(this, error));
                    break;
                }
            }

            _logger.LogDebug("Exited read loop for {Interactor}", this);
        }

        private void WriteQueuedMessages()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var message = _writeQueue.Take(_token);
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

            _logger.LogDebug("Exited read loop for {Interactor}", this);
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
            _stream.Close();
        }
    }
}
