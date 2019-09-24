#nullable enable

using System;

using Microsoft.Extensions.Logging;

using Prometheus;

using JetBlack.MessageBus.Distributor.Roles;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class InteractorManager
    {
        private readonly ILogger<InteractorManager> _logger;
        private readonly InteractorRepository _repository;
        private readonly Counter _interatorsFaulted = Metrics.CreateCounter("interactor_faults", "The number of interactor faults");
        private readonly Counter _authorizationRequests = Metrics.CreateCounter("authorization_requests", "The number of authorization requests");
        private readonly Counter _authorizationResponses = Metrics.CreateCounter("authorization_responses", "The number of authorization responses");

        public InteractorManager(DistributorRole distributorRole, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<InteractorManager>();
            _repository = new InteractorRepository(distributorRole);
        }

        internal EventHandler<AuthorizationResponseEventArg>? AuthorizationResponses;
        public event EventHandler<InteractorClosedEventArgs>? ClosedInteractors;
        public event EventHandler<InteractorFaultedEventArgs>? FaultedInteractors;

        public void AddInteractor(Interactor interactor)
        {
            _logger.LogInformation("Adding interactor: {Interactor}", interactor);

            _repository.Add(interactor);
        }

        public void CloseInteractor(Interactor interactor)
        {
            _logger.LogInformation("Closing interactor: {Interactor}", interactor);

            _repository.Remove(interactor);
            ClosedInteractors?.Invoke(this, new InteractorClosedEventArgs(interactor));
        }

        public void FaultInteractor(Interactor interactor, Exception error)
        {
            _logger.LogInformation("Faulting interactor: {Interactor}", interactor);
            _interatorsFaulted.Inc();

            _repository.Remove(interactor);
            FaultedInteractors?.Invoke(this, new InteractorFaultedEventArgs(interactor, error));
        }
        internal void RequestAuthorization(Interactor interactor, string feed, string topic)
        {
            _logger.LogDebug(
                "Requesting authorization Interactor={Interactor}, Feed={Feed}, Topic={Topic}",
                interactor,
                feed,
                topic);
            _authorizationRequests.Inc();

            if (!interactor.IsAuthorizationRequired(feed))
            {
                _logger.LogDebug("No authorization required");
                AcceptAuthorization(interactor, new AuthorizationResponse(interactor.Id, feed, topic, false, null));
                return;
            }

            var authorizationRequest = new AuthorizationRequest(
                interactor.Id,
                interactor.HostForFeed(feed),
                interactor.UserForFeed(feed),
                feed,
                topic);

            var authorizers = _repository.Find(feed, Role.Authorize);
            if (authorizers.Count == 0)
            {
                _logger.LogWarning("No authorizers for for feed {Feed}", feed);
                return;
            }

            foreach (var authorizer in authorizers)
            {
                try
                {
                    _logger.LogDebug("Requesting authorization from {Authorizer}", authorizer);
                    authorizer.SendMessage(authorizationRequest);
                }
                catch (Exception error)
                {
                    _logger.LogWarning(error, "Failed to send {Authorizer} message {Request}", authorizer, authorizationRequest);
                }
            }
        }

        internal void AcceptAuthorization(Interactor authorizer, AuthorizationResponse authorization)
        {
            _logger.LogDebug("Accepting an authorization response from {Authorizer} with {Authorization}.", authorizer, authorization);
            _authorizationResponses.Inc();

            var requestor = _repository.Find(authorization.ClientId);
            if (requestor == null)
            {
                _logger.LogWarning(
                    "Unable to queue an authorization response for unknown ClientId={ClientId} for Feed=\"{Feed}\", Topic=\"{Topic}\".",
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
