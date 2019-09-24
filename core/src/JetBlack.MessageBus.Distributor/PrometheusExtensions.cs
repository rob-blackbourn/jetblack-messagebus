using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prometheus;

namespace JetBlack.MessageBus.Distributor
{
    public static class PrometheusExtensions
    {
        public static Counter Get(this IDictionary<ITuple, Counter> counters, ITuple key, string name, string help, params string[] labelNames)
        {
            if (!counters.TryGetValue(key, out var counter))
            {
                counters.Add(key, counter = Metrics.CreateCounter(name, help, ExtendLabelNames(key, labelNames)));
            }
            return counter;
        }

        public static Counter Get(this IDictionary<string, Counter> counters, string key, string name, string help, params string[] labelNames)
        {
            if (!counters.TryGetValue(key, out var counter))
            {
                counters.Add(key, counter = Metrics.CreateCounter(name, help, ExtendLabelNames(key, labelNames)));
            }
            return counter;
        }

        public static Gauge Get(this IDictionary<ITuple, Gauge> counters, ITuple key, string name, string help, params string[] labelNames)
        {
            if (!counters.TryGetValue(key, out var gauge))
                counters.Add(key, gauge = Metrics.CreateGauge(name, help, ExtendLabelNames(key, labelNames)));
            return gauge;
        }

        public static Gauge Get(this IDictionary<string, Gauge> counters, string key, string name, string help, params string[] labelNames)
        {
            if (!counters.TryGetValue(key, out var gauge))
                counters.Add(key, gauge = Metrics.CreateGauge(name, help, ExtendLabelNames(key, labelNames)));
            return gauge;
        }

        private static string[] ExtendLabelNames(string labelName, string[] labelNames)
        {
            var labels = new string[1 + labelNames.Length];
            labels[0] = labelName;
            labelNames.CopyTo(labels, 1);
            return labels;
        }

        private static string[] ExtendLabelNames(ITuple key, string[] labelNames)
        {
            var labels = new string[key.Length + labelNames.Length];
            key.CopyTo(labels, 0);
            labelNames.CopyTo(labels, key.Length);
            return labels;
        }

        private static void CopyTo(this ITuple source, string[] dest, int index)
        {
            for (int i = 0; i < source.Length; ++i)
            {
                var value = source[i];
                if (value != null && value is string)
                    dest[i + index] = (string)value;
                else
                    throw new ApplicationException("Invalid type");
            }
        }

    }

    public class CounterDictionary
    {
        private readonly Dictionary<string, Counter> _counters = new Dictionary<string, Counter>();

        public CounterDictionary(string name, string help, params string[] labelNames)
        {
            Name = name;
            Help = help;
            LabelNames = labelNames;
        }

        public string Name { get; }
        public string Help { get; }
        public string[] LabelNames { get; }

        public Counter this[string key]
        {
            get => _counters.Get(key, Name, Help, LabelNames);
        }
    }

    public class GaugeDictionary
    {
        private readonly Dictionary<string, Gauge> _counters = new Dictionary<string, Gauge>();

        public GaugeDictionary(string name, string help, params string[] labelNames)
        {
            Name = name;
            Help = help;
            LabelNames = labelNames;
        }

        public string Name { get; }
        public string Help { get; }
        public string[] LabelNames { get; }

        public Gauge this[string key]
        {
            get => _counters.Get(key, Name, Help, LabelNames);
        }
    }


    public class GaugeDictionary2
    {
        private readonly Dictionary<ITuple, Gauge> _counters = new Dictionary<ITuple, Gauge>();

        public GaugeDictionary2(string name, string help, params string[] labelNames)
        {
            Name = name;
            Help = help;
            LabelNames = labelNames;
        }

        public string Name { get; }
        public string Help { get; }
        public string[] LabelNames { get; }

        public Gauge this[ITuple key]
        {
            get => _counters.Get(key, Name, Help, LabelNames);
        }
    }
}