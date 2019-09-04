#nullable enable

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace JetBlack.MessageBus.Distributor
{
    public class EventQueue<T> where T : EventArgs
    {
        private readonly ILogger<EventQueue<T>> _logger;
        private readonly CancellationToken _token;
        private readonly BlockingCollection<T> _interactorEventQueue = new BlockingCollection<T>();

        public EventQueue(ILoggerFactory loggerFactory, CancellationToken token)
        {
            _logger = loggerFactory.CreateLogger<EventQueue<T>>();
            _token = token;
        }

        public EventHandler<T>? OnItemDequeued;

        public void Enqueue(T item)
        {
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
        }
    }
}
