#nullable enable

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using log4net;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Interactors;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor
{
    public class Acceptor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Acceptor));

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
            CancellationToken token)
        {
            _endPoint = endPoint;
            _certificate = certificate;
            _authenticator = authenticator;
            _distributorRole = distributorRole;
            _eventQueue = eventQueue;
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
                    Log.Warn("Failed to accept connection", error);
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
                    _token);
                _eventQueue.Enqueue(new InteractorConnectedEventArgs(interactor));
            }
            catch (Exception error)
            {
                Log.Warn("Failed to create interactor", error);
            }
        }
    }
}
