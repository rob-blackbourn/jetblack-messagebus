#nullable enable

using System.Collections.Generic;

namespace JetBlack.MessageBus.Common.Linq
{
    /// <summary>
    /// A struct representing the head and tail of a collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements in a collection.</typeparam>
    public struct HeadAndTail<T>
    {
        /// <summary>
        /// The head of the collection.
        /// </summary>
        public T Head { get; }

        /// <summary>
        /// The tail of the collection.
        /// </summary>
        public IEnumerable<T> Tail { get; }

        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="head">The head of the collection.</param>
        /// <param name="tail">The tail of the collection.</param>
        public HeadAndTail(T head, IEnumerable<T> tail)
        {
            Head = head;
            Tail = tail;
        }
    }

    /// <summary>
    /// Extension methods for the head tail structure.
    /// </summary>
    public static class HeadAndTailExtensions
    {
        /// <summary>
        /// Construct a head and tail object from a collection.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <returns></returns>
        public static HeadAndTail<T> HeadAndTail<T>(this IEnumerable<T> source)
        {
            var e = source.GetEnumerator();
            e.MoveNext();
            var result = new HeadAndTail<T>(e.Current, e.Tail());
            e.Dispose();
            return result;
        }

        private static IEnumerable<T> Tail<T>(this IEnumerator<T> e)
        {
            while (e.MoveNext())
                yield return e.Current;
        }
    }
}
