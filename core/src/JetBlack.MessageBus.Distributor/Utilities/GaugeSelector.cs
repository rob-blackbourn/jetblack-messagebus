using System.Collections.Generic;

using Prometheus;

namespace JetBlack.MessageBus.Distributor.Utilities
{
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