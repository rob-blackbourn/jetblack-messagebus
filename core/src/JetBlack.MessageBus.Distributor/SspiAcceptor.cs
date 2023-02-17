using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Distributor.Utilities;
using System.Security.Authentication;

namespace JetBlack.MessageBus.Distributor
{
    public class SspiAcceptor
    {
        private readonly ILogger<Acceptor> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPEndPoint _endPoint;
        private readonly IAuthenticator _authenticator;
        private readonly DistributorRole _distributorRole;
        private readonly EventQueue<InteractorEventArgs> _eventQueue;
        private readonly CancellationToken _token;

        public SspiAcceptor(
            IPEndPoint endPoint,
            IAuthenticator authenticator,
            DistributorRole distributorRole,
            EventQueue<InteractorEventArgs> eventQueue,
            ILoggerFactory loggerFactory,
            CancellationToken token)
        {
            _endPoint = endPoint;
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
            _logger.LogInformation(
                "Listening to {EndPoint} using SSPI authentication.",
                _endPoint);

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
                    _logger.LogWarning(error, "Failed to accept a connection.");
                    throw;
                }
            }

            listener.Stop();
        }

        private void CreateInteractor(TcpClient tcpClient)
        {
            try
            {
                var interactor = Interactor.CreateSspi(
                    tcpClient,
                    _authenticator,
                    _distributorRole,
                    _eventQueue,
                    _loggerFactory,
                    _token);
                _eventQueue.Enqueue(new InteractorConnectedEventArgs(interactor));
            }
            catch (AuthenticationException error)
            {
                _logger.LogWarning(error, "Authentication failed.");
            }
            catch (Exception error)
            {
                _logger.LogWarning(error, "Failed to create interactor.");
            }
        }
    }
}
