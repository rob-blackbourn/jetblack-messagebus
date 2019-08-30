#nullable enable

using System;
using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.Linq
{
    /// <summary>
    /// Extension functions for collections which support List&lt;T&gt; or IList&lt;T&gt;
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Splits a list into sub-lists of a given size.
        /// </summary>
        /// <param name="items">The source list.</param>
        /// <param name="nSize">The number of items in the sub-lists</param>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <returns>A list of sub-lists of the given size, with a possible smaller final sub-list.</returns>
        public static List<List<T>> SplitList<T>(this List<T> items, int nSize = 100)
        {
            var list = new List<List<T>>();

            for (var i = 0; i < items.Count; i += nSize)
                list.Add(items.GetRange(i, Math.Min(nSize, items.Count - i)));

            return list;
        }

        /// <summary>
        /// Finds the index of an item in a collection.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="value">The value to find.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <returns>If the item is found the index of that item, otherwise -1.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> source, T value)
        {
            return IndexOf(source, value, (a, b) => Equals(a, b));
        }

        /// <summary>
        /// Finds the index of an item in a collection given a predicate.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="predicate">The predicate to use to find the index.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <returns>If the predicate is satisfied the index of that point, otherwise -1.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
        {
            for (var i = 0; i < source.Count; ++i)
                if (predicate(source[i]))
                    return i;
            return -1;
        }

        /// <summary>
        /// Finds the index of an item in a collection given a predicate and a secondary value.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="value">A secondayr value to be passed in to the predicate.</param>
        /// <param name="predicate">The predicate to use to find the index.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <returns>If the predicate is satisfied the index of that point, otherwise -1.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> source, T value, Func<T, T, bool> predicate)
        {
            for (var i = 0; i < source.Count; ++i)
                if (predicate(source[i], value))
                    return i;
            return -1;
        }
    }
}
