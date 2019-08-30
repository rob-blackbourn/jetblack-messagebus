#nullable enable

using System;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Messages;
using log4net;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class InteractorManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(InteractorManager));

        private readonly InteractorRepository _repository;

        public InteractorManager(DistributorRole distributorRole)
        {
            _repository = new InteractorRepository(distributorRole);
        }

        internal EventHandler<AuthorizationResponseEventArg>? AuthorizationResponses;
        public event EventHandler<InteractorClosedEventArgs>? ClosedInteractors;
        public event EventHandler<InteractorFaultedEventArgs>? FaultedInteractors;

        public void AddInteractor(Interactor interactor)
        {
            Log.Info($"Adding interactor: {interactor}");

            _repository.Add(interactor);
        }

        public void CloseInteractor(Interactor interactor)
        {
            Log.Info($"Closing interactor: {interactor}");

            _repository.Remove(interactor);
            ClosedInteractors?.Invoke(this, new InteractorClosedEventArgs(interactor));
        }

        public void FaultInteractor(Interactor interactor, Exception error)
        {
            Log.Info($"Faulting interactor: {interactor}");

            _repository.Remove(interactor);
            FaultedInteractors?.Invoke(this, new InteractorFaultedEventArgs(interactor, error));
        }
        internal void RequestAuthorization(Interactor interactor, string feed, string topic)
        {
            Log.Debug($"Requesting authorization Interactor={interactor}, Feed={feed}, Topic={topic}");

            if (!IsAuthorizationRequired(feed))
            {
                Log.Debug("No authorization required");
                AcceptAuthorization(interactor, new AuthorizationResponse(interactor.Id, feed, topic, false, null));
            }
            else
            {
                var authorizationRequest = new AuthorizationRequest(interactor.Id, interactor.Host, interactor.User, feed, topic);

                foreach (var authorizer in _repository.Find(feed, Role.Authorize))
                {
                    try
                    {
                        Log.Debug($"Requesting authorization from {authorizer}");
                        authorizer.SendMessage(authorizationRequest);
                    }
                    catch (Exception exception)
                    {
                        Log.Warn($"Failed to send {authorizer} message {authorizationRequest}", exception);
                    }
                }
            }
        }

        internal void AcceptAuthorization(Interactor authorizer, AuthorizationResponse message)
        {
            Log.Debug($"Accepting an authorization response from {authorizer} with {message}.");

            var requestor = _repository.Find(message.ClientId);
            if (requestor == null)
            {
                Log.Warn($"Unable to queue an authorization response for unknown ClientId={message.ClientId} for Feed=\"{message.Feed}\", Topic=\"{message.Topic}\".");
                return;
            }

            AuthorizationResponses?.Invoke(this, new AuthorizationResponseEventArg(authorizer, requestor, message));
        }

        private bool IsAuthorizationRequired(string feed)
        {
            return _repository.DistributorRole.IsAuthorizationRequiredForFeed(feed);
        }

        public void Dispose()
        {
            Log.Debug($"Disposing all interactors.");
            _repository.Dispose();
        }
    }
}
