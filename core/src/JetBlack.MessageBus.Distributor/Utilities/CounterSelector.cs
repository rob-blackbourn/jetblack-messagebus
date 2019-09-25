#nullable enable

using System.Collections.Generic;
using Prometheus;

namespace JetBlack.MessageBus.Distributor.Utilities
{
    public class CounterSelector
    {
        private readonly Counter _counter;
        private readonly string[] _labelValues;
        private readonly IDictionary<string, Counter.Child> _cache = new Dictionary<string, Counter.Child>();

        public CounterSelector(Counter counter, params string[] labelValues)
        {
            _counter = counter;
            _labelValues = labelValues;
        }

        public Counter.Child this[string key]
        {
            get
            {
                if (!_cache.TryGetValue(key, out var counter))
                    _cache.Add(key, counter = Create(key));
                return counter;
            }
        }

        private Counter.Child Create(string key)
        {
            var labelValues = new string[1 + _labelValues.Length];
            labelValues[0] = key;
            _labelValues.CopyTo(labelValues, 1);
            return _counter.WithLabels(labelValues);
        }
    }
}