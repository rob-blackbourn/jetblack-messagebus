#nullable enable

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace JetBlack.MessageBus.Distributor
{
    public class EventQueue<T> where T : EventArgs
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EventQueue<T>));

        private readonly CancellationToken _token;
        private readonly BlockingCollection<T> _interactorEventQueue = new BlockingCollection<T>();

        public EventQueue(CancellationToken token)
        {
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
                    Log.Info("The event queue has finished");
                    break;
                }
                catch (Exception error)
                {
                    Log.Error("The event queue has faulted", error);
                    break;
                }
            }

            Log.Info("Exited the event loop");
        }
    }
}
