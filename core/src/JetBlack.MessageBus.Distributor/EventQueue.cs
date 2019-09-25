#nullable enable

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prometheus;

namespace JetBlack.MessageBus.Distributor
{
    public class EventQueue<T> where T : EventArgs
    {
        private readonly ILogger<EventQueue<T>> _logger;
        private readonly CancellationToken _token;
        private readonly BlockingCollection<T> _interactorEventQueue = new BlockingCollection<T>();
        private readonly Counter _eventsEnqueued = Metrics.CreateCounter("messagebus_events_enqueued", "The number of events enqueued");
        private readonly Counter _eventsDequeued = Metrics.CreateCounter("messagebus_events_dequeued", "The number of events dequeued");
        private readonly Gauge _eventsQueueLength = Metrics.CreateGauge("messagebus_events_queue_length", "The number of events on the queue");

        public EventQueue(ILoggerFactory loggerFactory, CancellationToken token)
        {
            _logger = loggerFactory.CreateLogger<EventQueue<T>>();
            _token = token;
        }

        public EventHandler<T>? OnItemDequeued;

        public void Enqueue(T item)
        {
            _eventsEnqueued.Inc();
            _eventsQueueLength.Inc();
            _interactorEventQueue.Add(item, _token);
        }

        public void Start()
        {
            Task.Factory.StartNew(ProcessQueue, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ProcessQueue()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var item = _interactorEventQueue.Take(_token);
                    _eventsDequeued.Inc();
                    _eventsQueueLength.Dec();
                    OnItemDequeued?.Invoke(this, item);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("The event queue has finished");
                    break;
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "The event queue has faulted");
                    break;
                }
            }

            _logger.LogInformation("Exited the event loop");

            _eventsQueueLength.Set(0);
        }
    }
}
