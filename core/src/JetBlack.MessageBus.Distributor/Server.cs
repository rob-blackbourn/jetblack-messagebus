using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using Microsoft.Extensions.Logging;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Distributor.Notifiers;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Distributor.Subscribers;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor
{
    public class Server : IDisposable
    {
        private readonly ILogger<Server> _logger;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly Acceptor _acceptor;
        private readonly SspiAcceptor? _sspiAcceptor;
        private readonly Timer _heartbeatTimer;
        private readonly InteractorManager _interactorManager;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly NotificationManager _notificationManager;
        private readonly Interactor _heartbeatInteractor;

        public Server(
            IPEndPoint endPoint,
            IAuthenticator authenticator,
            X509Certificate2? certificate,
            DistributorRole distributorRole,
            IPEndPoint? sspiEndPoint,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Server>();

            _eventQueue = new EventQueue<InteractorEventArgs>(loggerFactory, _tokenSource.Token);
            _eventQueue.OnItemDequeued += OnInteractorEvent;

            _heartbeatTimer = new Timer(HeartbeatCallback);

            _acceptor = new Acceptor(
                endPoint,
                certificate,
                authenticator,
                distributorRole,
                _eventQueue,
                loggerFactory,
                _tokenSource.Token);

            _sspiAcceptor = sspiEndPoint == null
                ? null
                : new SspiAcceptor(
                    sspiEndPoint,
                    distributorRole,
                    _eventQueue,
                    loggerFactory,
                    _tokenSource.Token);

            _interactorManager = new InteractorManager(distributorRole, loggerFactory);

            _notificationManager = new NotificationManager(_interactorManager, loggerFactory);

            _subscriptionManager = new SubscriptionManager(
                _interactorManager,
                _notificationManager,
                loggerFactory);

            _heartbeatInteractor = new Interactor(
                new MemoryStream(),
                "Heartbeat",
                new RoleManager(new DistributorRole(Role.Publish, Role.Authorize | Role.Notify | Role.Subscribe, false, null), "localhost", "admin", null, null),
                _eventQueue,
                loggerFactory.CreateLogger<Interactor>(),
                _tokenSource.Token);
        }

        public void Start(TimeSpan heartbeatInterval)
        {
            _logger.LogInformation(
                "Starting the server (version \"{Version}\").",
                Assembly.GetExecutingAssembly().GetName().Version);

            _eventQueue.Start();
            _acceptor.Start();
            _sspiAcceptor?.Start();

            if (heartbeatInterval != TimeSpan.Zero)
                _heartbeatTimer.Change(heartbeatInterval, heartbeatInterval);

            _logger.LogInformation("The server is ready.");
        }

        private void OnInteractorEvent(object? sender, InteractorEventArgs args)
        {
            if (args is InteractorConnectedEventArgs)
                OnInteractorConnected((InteractorConnectedEventArgs)args);
            else if (args is InteractorErrorEventArgs)
                OnInteractorError((InteractorErrorEventArgs)args);
            else if (args is InteractorClosedEventArgs)
                OnInteractorClosed((InteractorClosedEventArgs)args);
            else if (args is InteractorMessageEventArgs)
                OnMessage((InteractorMessageEventArgs)args);
            else
                _logger.LogError("An unknown interactor event has been received.");
        }

        private static bool IsCloseException(Exception error)
        {
            if (error is EndOfStreamException)
                return true;
            var socketError = error.InnerException as SocketException;
            if (socketError != null && socketError.SocketErrorCode == SocketError.ConnectionReset)
                return true;

            return false;
        }

        private void OnInteractorConnected(InteractorConnectedEventArgs args)
        {
            _interactorManager.AddInteractor(args.Interactor);
            args.Interactor.Start();
        }

        private void OnInteractorClosed(InteractorClosedEventArgs args)
        {
            _interactorManager.CloseInteractor(args.Interactor);
        }

        private void OnInteractorError(InteractorErrorEventArgs args)
        {
            if (IsCloseException(args.Error))
                _interactorManager.CloseInteractor(args.Interactor);
            else
                _interactorManager.FaultInteractor(args.Interactor, args.Error);
        }

        private void OnMessage(InteractorMessageEventArgs args)
        {
            _logger.LogTrace(
                "Message received from {Sender} with {Message}.",
                args.Interactor,
                args.Message);

            switch (args.Message.MessageType)
            {
                case MessageType.AuthorizationResponse:
                    _interactorManager.AcceptAuthorization(args.Interactor, (AuthorizationResponse)args.Message);
                    break;

                case MessageType.SubscriptionRequest:
                    _subscriptionManager.RequestSubscription(args.Interactor, (SubscriptionRequest)args.Message);
                    break;

                case MessageType.MulticastData:
                    _subscriptionManager.SendMulticastData(args.Interactor, (MulticastData)args.Message);
                    break;

                case MessageType.UnicastData:
                    _subscriptionManager.SendUnicastData(args.Interactor, (UnicastData)args.Message);
                    break;

                case MessageType.NotificationRequest:
                    _notificationManager.RequestNotification(args.Interactor, (NotificationRequest)args.Message);
                    break;

                default:
                    _logger.LogError(
                        "Received unknown message type {MessageType} from interactor {Interactor}.",
                        args.Message.MessageType,
                        args.Interactor);
                    break;
            }
        }

        private void HeartbeatCallback(object? state)
        {
            _logger.LogTrace("Sending a heartbeat.");
            _eventQueue.Enqueue(new InteractorMessageEventArgs(_heartbeatInteractor, new MulticastData("__admin__", "heartbeat", true, null)));
        }

        public void Dispose()
        {
            _logger.LogInformation("Stopping the server.");

            _heartbeatTimer.Dispose();
            _heartbeatInteractor.Dispose();

            _tokenSource.Cancel();

            _logger.LogInformation("The server has stopped.");
        }
    }
}
