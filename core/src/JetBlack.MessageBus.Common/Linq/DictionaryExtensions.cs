#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBlack.MessageBus.Common.Collections.Generic;

namespace JetBlack.MessageBus.Common.Linq
{
    /// <summary>
    /// Extension methods for generic dictionaries.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Create a formatted string of a dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="indentation">The indentation step.</param>
        /// <param name="indentationLevel">The current level of indentation.</param>
        /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <returns>A formatted string</returns>
        public static string ToFormattedString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, int? indentation = null, int? indentationLevel = null)
        {
            return dictionary.ToFormattedString(indentation, indentationLevel ?? 0);
        }

        private static string ToFormattedString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, int? indentation, int indentationLevel)
        {
            if (dictionary == null)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var keyValuePair in dictionary)
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                    if (indentation.HasValue)
                        sb.AppendLine();
                }

                if (indentation.HasValue && indentationLevel > 0)
                    sb.Append(new string(' ', indentation.Value * indentationLevel));
                sb.Append('{').Append(keyValuePair.Key).Append(":");
                if (keyValuePair.Value is IDictionary<TKey, TValue>)
                    sb.Append(((IDictionary<TKey, TValue>)keyValuePair.Value).ToFormattedString(indentation, indentationLevel + 1));
                else
                    sb.Append(keyValuePair.Value);
                sb.Append('}');
            }

            if (sb.Length > 0 && indentation.HasValue)
                sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Construct a dictionary from an enumeration of key value pairs.
        /// </summary>
        /// <param name="source">The source enumeration</param>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <typeparam name="TValue">The type of the values</typeparam>
        /// <returns>The constructed dictionary</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            var dict = new Dictionary<TKey, TValue>();
            foreach (var item in source)
                dict.Add(item.Key, item.Value);
            return dict;
        }

        /// <summary>
        /// Creates a dictionary given a key and value selector which take an index.
        /// </summary>
        /// <param name="source">The source enumeration</param>
        /// <param name="keySelector">A key selector</param>
        /// <param name="valueSelector">A value selector</param>
        /// <typeparam name="TSource">The type of the elements in the source enumeration</typeparam>
        /// <typeparam name="TKey">The type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values</typeparam>
        /// <returns>The created dictionary</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, int, TKey> keySelector, Func<TSource, int, TValue> valueSelector)
        {
            return source.AsKeyValuePairs(keySelector, valueSelector).ToDictionary();
        }

        /// <summary>
        /// Creates an enumeration of key value pairs given key and value selector which take an index argument.
        /// </summary>
        /// <param name="source">The source enumeration</param>
        /// <param name="keySelector">A function to select the key given the source element and its index.</param>
        /// <param name="valueSelector">A function to select the value given the source element and its index.</param>
        /// <typeparam name="TSource">The type of the source elements</typeparam>
        /// <typeparam name="TKey">The type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values</typeparam>
        /// <returns>The created dictionary</returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> AsKeyValuePairs<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, int, TKey> keySelector, Func<TSource, int, TValue> valueSelector)
        {
            return source.Select((x, i) => new KeyValuePair<TKey, TValue>(keySelector(x, i), valueSelector(x, i)));
        }

        /// <summary>
        /// Creates a dictionary where the values are a collection.
        /// </summary>
        /// <param name="source">The source enumeration.</param>
        /// <param name="keySelector">A key selector.</param>
        /// <param name="valueSelector">A value selector.</param>
        /// <typeparam name="TSource">The type of the source elements</typeparam>
        /// <typeparam name="TKey">The type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values</typeparam>
        /// <returns>The created dictionary</returns>
        public static IDictionary<TKey, IList<TValue>> ToListDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, IList<TValue>>();

            foreach (var item in source)
            {
                var key = keySelector(item);
                var value = valueSelector(item);
                IList<TValue> list;
                if (!dictionary.TryGetValue(key, out list))
                    dictionary.Add(key, list = new List<TValue>());
                list.Add(value);
            }

            return dictionary;
        }

        /// <summary>
        /// Creates a dictionary from an enumeration of key value pairs where the values are a collection.
        /// </summary>
        /// <param name="source">The source enumeration.</param>
        /// <typeparam name="TKey">The type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values</typeparam>
        /// <returns>The created dictionary</returns>
        public static IDictionary<TKey, IList<TValue>> ToListDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            var dictionary = new Dictionary<TKey, IList<TValue>>();

            foreach (var item in source)
            {
                IList<TValue> list;
                if (!dictionary.TryGetValue(item.Key, out list))
                    dictionary.Add(item.Key, list = new List<TValue>());
                list.Add(item.Value);
            }

            return dictionary;
        }

        /// <summary>
        /// Given an enumeration of keys, find the matching values in the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to search.</param>
        /// <param name="keys">The keys to find.</param>
        /// <typeparam name="TKey">The type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values</typeparam>
        /// <returns>The matching key value pairs</returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> FindMatching<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
        {
            TValue value;
            foreach (var key in keys)
                if (dictionary.TryGetValue(key, out value))
                    yield return new KeyValuePair<TKey, TValue>(key, value);
        }

        /// <summary>
        /// Iterate over a dictionary applying the action.
        /// </summary>
        /// <param name="source">The source dictionary</param>
        /// <param name="action">The action to apply.</param>
        /// <typeparam name="TKey">The type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values</typeparam>
        public static void ForeEach<TKey, TValue>(this IDictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>> action)
        {
            foreach (var item in source)
                action(item);
        }

        /// <summary>
        /// Iterate over a dictionary applying the action including the an ordinal index.
        /// </summary>
        /// <param name="source">The source dictionary</param>
        /// <param name="action">The action to apply.</param>
        /// <typeparam name="TKey">The type of the dictionary keys</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values</typeparam>
        public static void ForeEach<TKey, TValue>(this IDictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>, int> action)
        {
            foreach (var item in source.Select((entry, i) => new { entry, i }))
                action(item.entry, item.i);
        }
    }
}
