using System;
using System.Collections;
using System.Collections.Generic;

using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class InteractorRepository : IDisposable, IEnumerable<Interactor>
    {
        private readonly IDictionary<Guid, Interactor> _interactors = new Dictionary<Guid, Interactor>();
        private readonly IDictionary<string, IDictionary<Role, HashSet<Interactor>>> _feedRoleInteractors = new Dictionary<string, IDictionary<Role, HashSet<Interactor>>>();

        internal InteractorRepository(DistributorRole distributorRole)
        {
            DistributorRole = distributorRole;
        }

        internal DistributorRole DistributorRole { get; }

        public void Add(Interactor interactor)
        {
            interactor.Metrics.Interactors.Inc();

            _interactors.Add(interactor.Id, interactor);
            AddFeedRoles(interactor);
        }

        public bool Remove(Interactor interactor)
        {
            interactor.Metrics.Interactors.Dec();

            RemoveFeedRoles(interactor);

            interactor.Dispose();

            return _interactors.Remove(interactor.Id);
        }


        internal Interactor? Find(Guid id)
        {
            if (_interactors.TryGetValue(id, out var requestor))
                return requestor;
            return null;
        }

        internal IReadOnlyCollection<Interactor> Find(string feed, Role role)
        {
            if (_feedRoleInteractors.TryGetValue(feed, out var roleInteractors))
            {
                if (roleInteractors.TryGetValue(role, out var interactors))
                    return interactors;
            }

            return new Interactor[0];
        }

        private void AddFeedRoles(Interactor interactor)
        {
            foreach (var feed in DistributorRole.FeedRoles.Keys)
            {
                if (!_feedRoleInteractors.TryGetValue(feed, out var roleInteractor))
                    _feedRoleInteractors.Add(feed, roleInteractor = new Dictionary<Role, HashSet<Interactor>>());

                var host = interactor.HostForFeed(feed);
                var user = interactor.UserForFeed(feed);

                foreach (var role in new[] { Role.Publish, Role.Subscribe, Role.Notify, Role.Authorize })
                {
                    if (DistributorRole.HasRole(host, user, feed, role))
                    {
                        if (!roleInteractor.TryGetValue(role, out var interactors))
                            roleInteractor.Add(role, interactors = new HashSet<Interactor>());

                        interactors.Add(interactor);
                    }
                }
            }
        }

        private void RemoveFeedRoles(Interactor interactor)
        {
            var feedsModified = new HashSet<string>();

            foreach (var feedRoleInteractors in _feedRoleInteractors)
            {
                var rolesModified = new HashSet<Role>();

                foreach (var roleInteractors in feedRoleInteractors.Value)
                {
                    if (roleInteractors.Value.Remove(interactor))
                        rolesModified.Add(roleInteractors.Key);
                }

                foreach (var role in rolesModified)
                {
                    if (feedRoleInteractors.Value[role].Count == 0)
                    {
                        feedRoleInteractors.Value.Remove(role);
                        feedsModified.Add(feedRoleInteractors.Key);
                    }
                }
            }

            foreach (var feed in feedsModified)
            {
                if (_feedRoleInteractors[feed].Count == 0)
                    _feedRoleInteractors.Remove(feed);
            }
        }

        public IEnumerator<Interactor> GetEnumerator()
        {
            return _interactors.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            foreach (var interactor in _interactors.Values)
            {
                interactor.Dispose();
            }

            _interactors.Clear();
        }
    }
}
