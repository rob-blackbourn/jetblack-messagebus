#nullable enable

using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.Linq
{
    /// <summary>
    /// Extensions functions for collections impleementing ICollection&lt;T&gt;.
    /// </summary>
    public static class CollectionExtensions
    {

        /// <summary>
        /// Add a range of items to a collection.
        /// </summary>
        /// <param name="collection">The existing collection.</param>
        /// <param name="items">The items to add to the collection.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }
    }
}
