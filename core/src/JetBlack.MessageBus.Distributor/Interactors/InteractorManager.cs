using System;

using Microsoft.Extensions.Logging;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class InteractorManager
    {
        private readonly ILogger<InteractorManager> _logger;
        private readonly InteractorRepository _repository;
        private readonly bool _isAuthorizationRequired;

        public InteractorManager(DistributorRole distributorRole, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<InteractorManager>();
            _repository = new InteractorRepository(distributorRole);
            _isAuthorizationRequired = distributorRole.IsAuthorizationRequired;
        }

        internal EventHandler<AuthorizationResponseEventArg>? AuthorizationResponses;
        public event EventHandler<InteractorClosedEventArgs>? ClosedInteractors;
        public event EventHandler<InteractorFaultedEventArgs>? FaultedInteractors;

        public void AddInteractor(Interactor interactor)
        {
            _logger.LogInformation("Adding interactor {Interactor}.", interactor);

            _repository.Add(interactor);
        }

        public void CloseInteractor(Interactor interactor)
        {
            _logger.LogInformation("Closing interactor {Interactor}.", interactor);

            _repository.Remove(interactor);
            ClosedInteractors?.Invoke(this, new InteractorClosedEventArgs(interactor));
        }

        public void FaultInteractor(Interactor interactor, Exception error)
        {
            _logger.LogInformation("Faulting interactor {Interactor}.", interactor);

            interactor.Metrics.Faulted.Inc();

            _repository.Remove(interactor);
            FaultedInteractors?.Invoke(this, new InteractorFaultedEventArgs(interactor, error));
        }
        internal void RequestAuthorization(Interactor interactor, string feed, string topic)
        {
            _logger.LogDebug(
                "Requesting authorization for {Interactor} on feed \"{Feed}\" with topic \"{Topic}\".",
                interactor,
                feed,
                topic);

            interactor.Metrics.AuthorizationRequests.Inc();

            if (!_isAuthorizationRequired)
            {
                _logger.LogDebug("No authorization is required.");
                AcceptAuthorization(interactor, new AuthorizationResponse(interactor.Id, feed, topic, false, null));
                return;
            }

            var authorizationRequest = new AuthorizationRequest(
                interactor.Id,
                interactor.Host,
                interactor.User,
                feed,
                topic);

            var authorizers = _repository.Find(feed, Role.Authorize);
            if (authorizers.Count == 0)
            {
                _logger.LogWarning(
                    "No authorizers found for feed \"{Feed}\".",
                    feed);
                return;
            }

            foreach (var authorizer in authorizers)
            {
                try
                {
                    _logger.LogDebug(
                        "Requesting authorization from {Authorizer}.",
                        authorizer);
                    authorizer.SendMessage(authorizationRequest);
                }
                catch (Exception error)
                {
                    _logger.LogWarning(
                        error,
                        "Failed to send {Authorizer} message {Request}.",
                        authorizer,
                        authorizationRequest);
                }
            }
        }

        internal void AcceptAuthorization(Interactor authorizer, AuthorizationResponse authorization)
        {
            _logger.LogDebug(
                "Accepting an authorization response from {Authorizer} with {Authorization}.",
                authorizer,
                authorization);

            authorizer.Metrics.AuthorizationResponses.Inc();

            var requestor = _repository.Find(authorization.ClientId);
            if (requestor == null)
            {
                _logger.LogWarning(
                    "Unable to queue an authorization response for unknown client {ClientId} for feed \"{Feed}\" with topic \"{Topic}\".",
                    authorization.ClientId,
                    authorization.Feed,
                    authorization.Topic);
                return;
            }

            var hasAuthorization = requestor.HasAuthorization(authorization.Feed, authorization.Topic);
            if (!hasAuthorization)
                requestor.SetAuthorization(authorization.Feed, authorization.Topic, authorization);

            AuthorizationResponses?.Invoke(this, new AuthorizationResponseEventArg(authorizer, requestor, authorization, !hasAuthorization));
        }

        public void Dispose()
        {
            _logger.LogDebug("Disposing all interactors.");
            _repository.Dispose();
        }
    }
}
