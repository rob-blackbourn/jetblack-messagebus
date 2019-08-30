#nullable enable

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using log4net;

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
        private static readonly ILog Log = LogManager.GetLogger(typeof(Server));

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly Acceptor _acceptor;
        private readonly Timer _heartbeatTimer;
        private readonly InteractorManager _interactorManager;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly NotificationManager _notificationManager;
        private readonly Interactor _heartbeatInteractor;

        public Server(
            IPEndPoint endPoint,
            IAuthenticator authenticator,
            X509Certificate2? certificate,
            DistributorRole distributorRole)
        {
            _eventQueue = new EventQueue<InteractorEventArgs>(_cancellationTokenSource.Token);
            _eventQueue.OnItemDequeued += OnInteractorEvent;

            _heartbeatTimer = new Timer(HeartbeatCallback);

            _acceptor = new Acceptor(
                endPoint,
                certificate,
                authenticator,
                distributorRole,
                _eventQueue,
                _cancellationTokenSource.Token);

            _interactorManager = new InteractorManager(distributorRole);

            _notificationManager = new NotificationManager(_interactorManager);

            _subscriptionManager = new SubscriptionManager(_interactorManager, _notificationManager);

            _heartbeatInteractor = new Interactor(
                new MemoryStream(),
                "localhost",
                "admin",
                new RoleManager(new DistributorRole(Role.Publish, Role.Authorize | Role.Notify | Role.Subscribe, false, null), string.Empty, string.Empty),
                _eventQueue,
                _cancellationTokenSource.Token);
        }

        public void Start(TimeSpan heartbeatInterval)
        {
            Log.Info($"Starting server version {Assembly.GetExecutingAssembly().GetName().Version}");

            _eventQueue.Start();
            _acceptor.Start();

            if (heartbeatInterval != TimeSpan.Zero)
                _heartbeatTimer.Change(heartbeatInterval, heartbeatInterval);

            Log.Info("Server started");
        }

        private void OnInteractorEvent(object? sender, InteractorEventArgs args)
        {
            if (args is InteractorConnectedEventArgs)
                OnInteractorConnected((InteractorConnectedEventArgs)args);
            else if (args is InteractorMessageEventArgs)
                OnMessage((InteractorMessageEventArgs)args);
            else if (args is InteractorErrorEventArgs)
                OnInteractorError((InteractorErrorEventArgs)args);
        }

        private void OnInteractorConnected(InteractorConnectedEventArgs args)
        {
            _interactorManager.AddInteractor(args.Interactor);
            args.Interactor.Start();
        }

        private void OnInteractorError(InteractorErrorEventArgs args)
        {
            if (args.Error is EndOfStreamException)
                _interactorManager.CloseInteractor(args.Interactor);
            else
                _interactorManager.FaultInteractor(args.Interactor, args.Error);
        }

        private void OnMessage(InteractorMessageEventArgs args)
        {
            Log.Debug($"OnMessage(sender={args.Interactor}, message={args.Message}");

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
                    Log.Warn($"Received unknown message type {args.Message.MessageType} from interactor {args.Interactor}.");
                    break;
            }
        }

        private void HeartbeatCallback(object? state)
        {
            Log.Debug("Sending heartbeat");
            _eventQueue.Enqueue(new InteractorMessageEventArgs(_heartbeatInteractor, new MulticastData("__admin__", "heartbeat", true, null)));
        }

        public void Dispose()
        {
            Log.Info("Stopping server");

            _heartbeatTimer.Dispose();

            _cancellationTokenSource.Cancel();

            Log.Info("Server stopped");
        }
    }
}
