#nullable enable

using System;

using Prometheus;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class InteractorMetrics
    {
        public InteractorMetrics(string host, string user, Guid id)
        {
            var labelValues = new string[] { host, user, id.ToString() };

            ReadsReceived = _Metrics.Instance.ReadsReceived.WithLabels(labelValues);
            WritesSent = _Metrics.Instance.WritesSent.WithLabels(labelValues);
            WriteQueueLength = _Metrics.Instance.WriteQueueLength.WithLabels(labelValues);
            Faulted = _Metrics.Instance.Faulted;
            AuthorizationRequests = _Metrics.Instance.AuthorizationRequests.WithLabels(labelValues);
            AuthorizationResponses = _Metrics.Instance.AuthorizationResponses.WithLabels(labelValues);
            Interactors = _Metrics.Instance.Interactors;
            ForwardedSubscriptions = new CounterSelector(
                _Metrics.Instance.ForwardedSubscriptions,
                _Metrics.Host, _Metrics.User, _Metrics.Id);
            FeedRequests = new GaugeSelector(
                _Metrics.Instance.FeedRequests,
                _Metrics.Host, _Metrics.User, _Metrics.Id);
            UnicastMessages = new CounterSelector(
                _Metrics.Instance.UnicastMessages,
                _Metrics.Host, _Metrics.User, _Metrics.Id);
            MulticastMessages = new CounterSelector(
                _Metrics.Instance.MulticastMessages,
                _Metrics.Host, _Metrics.User, _Metrics.Id);
            Subscribers = new GaugeSelector(
                _Metrics.Instance.Subscribers,
                _Metrics.Host, _Metrics.User, _Metrics.Id);
        }

        public Counter.Child ReadsReceived { get; }
        public Counter.Child WritesSent { get; }
        public Gauge.Child WriteQueueLength { get; }
        public Counter Faulted { get; }
        public Counter.Child AuthorizationRequests { get; }
        public Counter.Child AuthorizationResponses { get; }
        public Gauge Interactors { get; }
        public CounterSelector ForwardedSubscriptions { get; }
        public GaugeSelector FeedRequests { get; }
        public CounterSelector UnicastMessages { get; }
        public CounterSelector MulticastMessages { get; }
        public GaugeSelector Subscribers { get; }

        private class _Metrics
        {
            public const string Feed = "feed", Host = "host", User = "user", Id = "id";

            private static _Metrics? _instance;

            public static _Metrics Instance => _instance ?? (_instance = new _Metrics());

            public _Metrics()
            {
                ReadsReceived = Metrics.CreateCounter(
                    "messagebus_reads",
                    "The number of read messages read by an interactor",
                    Host, User, Id);

                WritesSent = Metrics.CreateCounter(
                   "messagebus_writes",
                   "The number of write messages sent from an interactor",
                   Host, User, Id);

                WriteQueueLength = Metrics.CreateGauge(
                   "messagebus_write_queue_length",
                   "The number of messages on an interactor write queue",
                   Host, User, Id);

                Faulted = Metrics.CreateCounter(
                    "messagebus_interactor_faults",
                    "The number of interactor faults",
                    Host, User, Id);

                AuthorizationRequests = Metrics.CreateCounter(
                    "messagebus_authorization_requests",
                    "The number of authorization requests",
                    Host, User, Id);

                AuthorizationResponses = Metrics.CreateCounter(
                    "messagebus_authorization_responses",
                    "The number of authorization responses",
                    Host, User, Id);

                Interactors = Metrics.CreateGauge(
                    "messagebus_interactors",
                    "The number of interactors",
                    Host, User, Id);

                ForwardedSubscriptions = Metrics.CreateCounter(
                    "messagebus_forwarded_subscriptions",
                    "The number of forwarded subscriptions",
                    Feed, Host, User, Id);

                FeedRequests = Metrics.CreateGauge(
                    "messagebus_notification_requests",
                    "The number of notification requests",
                    Feed, Host, User, Id);

                UnicastMessages = Metrics.CreateCounter(
                    "messagebus_published_unicast_messages",
                    "The number of unicast messages sent",
                    Feed, Host, User, Id);

                MulticastMessages = Metrics.CreateCounter(
                    "messagebus_published_multicast_messages",
                    "The number of multicast messages sent",
                    Feed, Host, User, Id);

                Subscribers = Metrics.CreateGauge(
                    "messagebus_subscribers",
                    "The number of subscribers",
                    Feed, Host, User, Id);
            }

            public Counter ReadsReceived { get; }
            public Counter WritesSent { get; }
            public Gauge WriteQueueLength { get; }
            public Counter Faulted { get; }
            public Counter AuthorizationRequests { get; }
            public Counter AuthorizationResponses { get; }
            public Gauge Interactors { get; }
            public Counter ForwardedSubscriptions { get; }
            public Gauge FeedRequests { get; }
            public Counter UnicastMessages { get; }
            public Counter MulticastMessages { get; }
            public Gauge Subscribers { get; }
        }
    }
}