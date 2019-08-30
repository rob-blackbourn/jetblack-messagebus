#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace JetBlack.MessageBus.Common.Linq
{
    /// <summary>
    /// Extensions for collections which support IEnumerable&lt;T&gt;.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Iterates over each item in the collection invoking the supplied action.
        /// </summary>
        /// <param name="collection">The collection over which to iterate</param>
        /// <param name="action">The action to perform on each item in the collection.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        /// <summary>
        /// Iterates over each item in the collection invoking the supplied action wi the item and the index.
        /// </summary>
        /// <param name="collection">The collection over which to iterate</param>
        /// <param name="action">The action to perform on each item in the collection together with the index of the item in the collection.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            int i = 0;
            foreach (var item in collection)
                action(item, i++);
        }

        /// <summary>
        /// Splits the input collection into collections of a given size.
        /// </summary>
        /// <param name="collection">The source collection.</param>
        /// <param name="batchSize">The size of the sub collection</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <returns>A collection of sub-collections of of given size, with a possible tail collection of a smaller size.</returns>
        public static IEnumerable<IList<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (var item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }

        /// <summary>
        /// Aggregates a collection in batches.
        /// </summary>
        /// <param name="collection">The source collection.</param>
        /// <param name="batchSize">The size of the batch.</param>
        /// <param name="func">The function to be applied to the batch.</param>
        /// <typeparam name="TSource">The type of the items in the collection.</typeparam>
        /// <returns>An enumeration of aggregated values.</returns>
        public static IEnumerable<TSource> BatchAggregate<TSource>(this IEnumerable<TSource> collection, int batchSize,
            Func<TSource, TSource, TSource> func)
        {
            return collection.Batch(batchSize).Select(inner => inner.Aggregate(func));
        }

        /// <summary>
        /// Aggregates a collection in batches, transforming the type of the items.
        /// </summary>
        /// <param name="collection">The source collection</param>
        /// <param name="batchSize">The number of items in each sub collection.</param>
        /// <param name="seed">The initial value to pass to the aggregator for each batch.</param>
        /// <param name="func">The function to use when aggregating a batch.</param>
        /// <typeparam name="TSource">The type of the items in the source collection.</typeparam>
        /// <typeparam name="TAccumulate">The type of the items in the aggregated enumeration.</typeparam>
        /// <returns>An enumeration of the aggregations of the batches</returns>
        public static IEnumerable<TAccumulate> BatchAggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> collection, int batchSize, TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            return collection.Batch(batchSize).Select(inner => inner.Aggregate(seed, func));
        }

        /// <summary>
        /// Aggregates a collection in batches, transforming the type of the items, then projecting.
        /// </summary>
        /// <param name="collection">The source collection</param>
        /// <param name="batchSize">The number of items in each sub collection.</param>
        /// <param name="seed">The initial value to pass to the aggregator for each batch.</param>
        /// <param name="func">The function to use when aggregating a batch.</param>
        /// <param name="resultSelector">A function to project the result of an aggregation.</param>
        /// <typeparam name="TSource">The type of the items in the source collection.</typeparam>
        /// <typeparam name="TAccumulate">The type of the items in the aggregated enumeration.</typeparam>
        /// <typeparam name="TResult">The return type of the projection.</typeparam>
        /// <returns>An enumeration of the aggregations of the batches</returns>
        public static IEnumerable<TResult> BatchAggregate<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> collection, int batchSize, TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return collection.Batch(batchSize).Select(inner => inner.Aggregate(seed, func, resultSelector));
        }

        /// <summary>
        /// Applies a condition to each item of a collection, returning an enumeration of the selected items.
        /// </summary>
        /// <param name="source">The source collection</param>
        /// <param name="predicate">The predicate to apply to each item.</param>
        /// <param name="selector">A selector function to apply to items which satisfy the predicate.</param>
        /// <typeparam name="TSource">The type of the items in the source collection.</typeparam>
        /// <typeparam name="TResult">The return type of the projection.</typeparam>
        /// <returns></returns>
        public static IEnumerable<TResult> If<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
        {
            foreach (var item in source)
                if (predicate(item))
                    yield return selector(item);
        }

        /// <summary>
        /// Creates an ISet from a given enumeration.
        /// </summary>
        /// <param name="values">The values from which to create the set.</param>
        /// <param name="factory">An optional factory to create the set.</param>
        /// <typeparam name="T">The type of the items in the source enumeration and the resultant set.</typeparam>
        /// <returns>A set of the items provided.</returns>
        public static ISet<T> ToSet<T>(this IEnumerable<T> values, Func<IEnumerable<T>, ISet<T>>? factory = null)
        {
            return factory == null ? new HashSet<T>(values) : factory(values);
        }

        /// <summary>
        /// Returns a rolling window of a given size.
        /// </summary>
        /// <param name="items">The source collection.</param>
        /// <param name="size">The size of the rollowing window.</param>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <returns>A rolling window of the given size.</returns>
        public static IEnumerable<IEnumerable<T>> RollingWindow<T>(this IEnumerable<T> items, int size)
        {
            var list = new Queue<T>();

            foreach (var item in items)
            {
                list.Enqueue(item);
                if (list.Count == size)
                {
                    yield return list;
                    list.Dequeue();
                }
            }
        }

        /// <summary>
        /// Fills missing values in aq source collection.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="keys">The keys expected in the source collection.</param>
        /// <param name="keySelector">A selector function applied to each key in the keys collection.</param>
        /// <param name="valueSelector">A selector function to be applied to each item in the source collection.</param>
        /// <param name="defaultValue">The value to be returned for keys not found in the source collection.</param>
        /// <typeparam name="TItem">The type of the items in the source collection.</typeparam>
        /// <typeparam name="TKey">The type of the items in the keys collection.</typeparam>
        /// <typeparam name="TValue">Type type of the result of the value selector.</typeparam>
        /// <returns>A collection of key value pairs where the keys match the supplied keys collection, and the value is derived from the source collection, otherwise filled with the default value.</returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> Fill<TItem, TKey, TValue>(
            this IEnumerable<TItem> source,
            IEnumerable<TKey> keys,
            Func<TItem, TKey> keySelector,
            Func<TItem, TValue> valueSelector,
            Func<TKey, TValue> defaultValue)
        {
            using (var enumerator = source.GetEnumerator())
            {
                var hasValue = enumerator.MoveNext();

                foreach (var key in keys)
                {
                    if (hasValue && key != null && key.Equals(keySelector(enumerator.Current)))
                    {
                        yield return new KeyValuePair<TKey, TValue>(key, valueSelector(enumerator.Current));
                        hasValue = enumerator.MoveNext();
                    }
                    else
                    {
                        yield return new KeyValuePair<TKey, TValue>(key, defaultValue(key));
                    }
                }
            }
        }

        /// <summary>
        /// Applies a specified function to the corresponding elements of three sequences, producing a sequence of the results.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="third">The third sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the three sequences.</param>
        /// <returns>An IEnumerable&lt;TResult&gt; that contains merged elements of three input sequences.</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TFirst, TSecond, TThird, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (third == null) throw new ArgumentNullException(nameof(third));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ZipIterator(first, second, third, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TThird, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TFirst, TSecond, TThird, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
            using (IEnumerator<TSecond> e2 = second.GetEnumerator())
            using (IEnumerator<TThird> e3 = third.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                    yield return resultSelector(e1.Current, e2.Current, e3.Current);
        }

        /// <summary>
        /// Applies a specified function to the corresponding elements of four sequences, producing a sequence of the results.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
        /// <typeparam name="TFourth">The type of the elements of the fourth input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="third">The third sequence to merge.</param>
        /// <param name="fourth">The fourth sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the four sequences.</param>
        /// <returns>An IEnumerable&lt;TResult&gt; that contains merged elements of four input sequences.</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, Func<TFirst, TSecond, TThird, TFourth, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (third == null) throw new ArgumentNullException(nameof(third));
            if (fourth == null) throw new ArgumentNullException(nameof(fourth));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ZipIterator(first, second, third, fourth, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TThird, TFourth, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, Func<TFirst, TSecond, TThird, TFourth, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
            using (IEnumerator<TSecond> e2 = second.GetEnumerator())
            using (IEnumerator<TThird> e3 = third.GetEnumerator())
            using (IEnumerator<TFourth> e4 = fourth.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext() && e4.MoveNext())
                    yield return resultSelector(e1.Current, e2.Current, e3.Current, e4.Current);
        }

        /// <summary>
        /// Applies a specified function to the corresponding elements of five sequences, producing a sequence of the results.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
        /// <typeparam name="TFourth">The type of the elements of the fourth input sequence.</typeparam>
        /// <typeparam name="TFifth">The type of the elements of the fifth input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="third">The third sequence to merge.</param>
        /// <param name="fourth">The fourth sequence to merge.</param>
        /// <param name="fifth">The fourth sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the five sequences.</param>
        /// <returns>An IEnumerable&lt;TResult&gt; that contains merged elements of five input sequences.</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TFifth, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, IEnumerable<TFifth> fifth, Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (third == null) throw new ArgumentNullException(nameof(third));
            if (fourth == null) throw new ArgumentNullException(nameof(fourth));
            if (fifth == null) throw new ArgumentNullException(nameof(fifth));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return ZipIterator(first, second, third, fourth, fifth, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TThird, TFourth, TFifth, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, IEnumerable<TFourth> fourth, IEnumerable<TFifth> fifth, Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> resultSelector)
        {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
            using (IEnumerator<TSecond> e2 = second.GetEnumerator())
            using (IEnumerator<TThird> e3 = third.GetEnumerator())
            using (IEnumerator<TFourth> e4 = fourth.GetEnumerator())
            using (IEnumerator<TFifth> e5 = fifth.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext() && e4.MoveNext() && e5.MoveNext())
                    yield return resultSelector(e1.Current, e2.Current, e3.Current, e4.Current, e5.Current);
        }

        /// <summary>
        /// A depth first traversal of a tree structure.
        /// </summary>
        /// <param name="item">The item node of the tree</param>
        /// <param name="childSelector">A function which returns the childSelector of an item.</param>
        /// <typeparam name="T">The type of the nodes in the tree.</typeparam>
        /// <returns>An enumeration of the nodes</returns>
        public static IEnumerable<T> DepthFirstTraverse<T>(this T item, Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>();
            stack.Push(item);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next).Reverse())
                    stack.Push(child);
            }
        }

        /// <summary>
        /// A breadth first traversal of a tree structure
        /// </summary>
        /// <param name="item">The item node of the tree</param>
        /// <param name="childSelector">A function which returns the childSelector of an item.</param>
        /// <typeparam name="T">The type of the nodes in the tree.</typeparam>
        /// <returns>An enumeration of the nodes</returns>
        public static IEnumerable<T> BreadthFirstTraversal<T>(this T item, Func<T, IEnumerable<T>> childSelector)
        {
            var q = new Queue<T>();
            q.Enqueue(item);
            while (q.Count > 0)
            {
                var current = q.Dequeue();
                yield return current;
                foreach (var child in childSelector(current))
                    q.Enqueue(child);
            }
        }
    }
}
