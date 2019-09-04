#nullable enable

using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Distributor.Roles;
using Microsoft.Extensions.Logging;

namespace JetBlack.MessageBus.Distributor
{
    public class Acceptor
    {
        private readonly ILogger<Acceptor> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPEndPoint _endPoint;
        private readonly X509Certificate2? _certificate;
        private readonly IAuthenticator _authenticator;
        private readonly DistributorRole _distributorRole;
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly CancellationToken _token;

        public Acceptor(
            IPEndPoint endPoint,
            X509Certificate2? certificate,
            IAuthenticator authenticator,
            DistributorRole distributorRole,
            EventQueue<InteractorEventArgs> eventQueue,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            _endPoint = endPoint;
            _certificate = certificate;
            _authenticator = authenticator;
            _distributorRole = distributorRole;
            _eventQueue = eventQueue;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Acceptor>();
            _token = token;
        }

        public void Start()
        {
            Task.Factory.StartNew(
                Accept,
                _token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void Accept()
        {
            _logger.LogInformation("Listening on {EndPoint}", _endPoint);

            var listener = new TcpListener(_endPoint);
            listener.Start();

            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = listener.AcceptTcpClient();
                    Task.Run(() => CreateInteractor(tcpClient));
                }
                catch (Exception error)
                {
                    _logger.LogWarning(error, "Failed to accept connection");
                    throw;
                }
            }

            listener.Stop();
        }

        private void CreateInteractor(TcpClient tcpClient)
        {
            try
            {
                var interactor = Interactor.Create(
                    tcpClient,
                    _certificate,
                    _authenticator,
                    _distributorRole,
                    _eventQueue,
                    _loggerFactory,
                    _token);
                _eventQueue.Enqueue(new InteractorConnectedEventArgs(interactor));
            }
            catch (Exception error)
            {
                _logger.LogWarning(error, "Failed to create interactor");
            }
        }
    }
}
