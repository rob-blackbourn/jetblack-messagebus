using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.Collections.Generic
{
    /// <summary>
    /// A set of paris whether the first is the key to the second, and the second is the key to the first.
    /// </summary>
    /// <typeparam name="TFirst">The type of the first value.</typeparam>
    /// <typeparam name="TSecond">The type of the second value</typeparam>
    public class TwoWaySet<TFirst, TSecond>
    {
        private readonly IDictionary<TFirst, ISet<TSecond>> _firstToSeconds = new Dictionary<TFirst, ISet<TSecond>>();
        private readonly IDictionary<TSecond, ISet<TFirst>> _secondToFirsts = new Dictionary<TSecond, ISet<TFirst>>();

        /// <summary>
        /// Adds the value pair.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="second">The second value.</param>
        public void Add(TFirst first, TSecond second)
        {
            ISet<TSecond> seconds;
            if (!_firstToSeconds.TryGetValue(first, out seconds))
                _firstToSeconds.Add(first, seconds = new HashSet<TSecond>());
            seconds.Add(second);

            ISet<TFirst> firsts;
            if (!_secondToFirsts.TryGetValue(second, out firsts))
                _secondToFirsts.Add(second, firsts = new HashSet<TFirst>());
            firsts.Add(first);
        }

        /// <summary>
        /// Adds the value pair.
        /// </summary>
        /// <param name="second">The second value.</param>
        /// <param name="first">The first value.</param>
        public void Add(TSecond second, TFirst first)
        {
            Add(first, second);
        }

        /// <summary>
        /// Find if the set contains the given key.
        /// </summary>
        /// <param name="first">The value to find</param>
        /// <returns>If the set contains the value <c>true</c>, otherwise <c>false</c>.</returns>
        public bool ContainsKey(TFirst first)
        {
            return _firstToSeconds.ContainsKey(first);
        }

        /// <summary>
        /// Find if the set contains the given key.
        /// </summary>
        /// <param name="second">The value to find</param>
        /// <returns>If the set contains the value <c>true</c>, otherwise <c>false</c>.</returns>
        public bool ContainsKey(TSecond second)
        {
            return _secondToFirsts.ContainsKey(second);
        }

        /// <summary>
        /// Attempt to get values given a key
        /// </summary>
        /// <param name="first">The key</param>
        /// <param name="seconds">A set of values matching the key</param>
        /// <returns>If the key exists <c>true</c>, otherwise <c>false</c>.</returns>
        public bool TryGetValue(TFirst first, out ISet<TSecond> seconds)
        {
            return _firstToSeconds.TryGetValue(first, out seconds);
        }

        /// <summary>
        /// Attempt to get values given a key
        /// </summary>
        /// <param name="second">The key</param>
        /// <param name="firsts">A set of values matching the key</param>
        /// <returns>If the key exists <c>true</c>, otherwise <c>false</c>.</returns>
        public bool TryGetValue(TSecond second, out ISet<TFirst> firsts)
        {
            return _secondToFirsts.TryGetValue(second, out firsts);
        }

        /// <summary>
        /// Remove the values given a key.
        /// </summary>
        /// <param name="first">The key</param>
        /// <returns>An enumeration of removed values.</returns>
        public IEnumerable<TSecond>? Remove(TFirst first)
        {
            if (!_firstToSeconds.TryGetValue(first, out var seconds))
                return null;

            var secondsWithoutFirsts = new HashSet<TSecond>();

            foreach (var second in seconds)
            {
                var firsts = _secondToFirsts[second];
                firsts.Remove(first);
                if (firsts.Count == 0)
                {
                    _secondToFirsts.Remove(second);
                    secondsWithoutFirsts.Add(second);
                }
            }

            _firstToSeconds.Remove(first);

            return secondsWithoutFirsts;
        }


        /// <summary>
        /// Remove the values given a key.
        /// </summary>
        /// <param name="second">The key</param>
        /// <returns>An enumeration of removed values.</returns>
        public IEnumerable<TFirst>? Remove(TSecond second)
        {
            if (!_secondToFirsts.TryGetValue(second, out var firsts))
                return null;

            var firstsWithoutSeconds = new HashSet<TFirst>();

            foreach (var first in firsts)
            {
                var seconds = _firstToSeconds[first];
                seconds.Remove(second);
                if (seconds.Count == 0)
                {
                    _firstToSeconds.Remove(first);
                    firstsWithoutSeconds.Add(first);
                }
            }

            _secondToFirsts.Remove(second);

            return firstsWithoutSeconds;
        }

        /// <summary>
        /// The keys from the perspective of the first type.
        /// </summary>
        public ICollection<TFirst> Firsts => _firstToSeconds.Keys;

        /// <summary>
        /// The keys from the perspective of the second type.
        /// </summary>
        public ICollection<TSecond> Seconds => _secondToFirsts.Keys;
    }
}
