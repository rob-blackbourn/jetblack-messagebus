#nullable enable

using System.Collections.Generic;
using Prometheus;

namespace JetBlack.MessageBus.Distributor
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

    public class GaugeSelector
    {
        private readonly Gauge _gauge;
        private readonly string[] _labelValues;
        private readonly IDictionary<string, Gauge.Child> _cache = new Dictionary<string, Gauge.Child>();

        public GaugeSelector(Gauge gauge, params string[] labelValues)
        {
            _gauge = gauge;
            _labelValues = labelValues;
        }

        public Gauge.Child this[string key]
        {
            get
            {
                if (!_cache.TryGetValue(key, out var gauge))
                    _cache.Add(key, gauge = Create(key));
                return gauge;
            }
        }

        private Gauge.Child Create(string key)
        {
            var labelValues = new string[1 + _labelValues.Length];
            labelValues[0] = key;
            _labelValues.CopyTo(labelValues, 1);
            return _gauge.WithLabels(labelValues);
        }
    }
}