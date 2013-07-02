using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SomethingBlue.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs a recursive traversal of the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="getChildren">The get children.</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren)
        {
            if (null == source)
                throw new ArgumentNullException("source");

            if (null == getChildren)
                return source;

            return TraverseIterator(source, getChildren);
        }

        private static IEnumerable<T> TraverseIterator<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren)
        {
            var stack = new Stack<IEnumerator<T>>();
            try
            {
                stack.Push(source.GetEnumerator());
                while (0 != stack.Count)
                {
                    var iter = stack.Peek();
                    if (iter.MoveNext())
                    {
                        T current = iter.Current;
                        yield return current;

                        var children = getChildren(current);
                        if (null != children)
                            stack.Push(children.GetEnumerator());
                    }
                    else
                    {
                        iter.Dispose();
                        stack.Pop();
                    }
                }
            }
            finally
            {
                while (0 != stack.Count)
                    stack.Pop().Dispose();
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var e in enumerable)
                action(e);
        }

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> predicate)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            using (var firstEnumerator = first.GetEnumerator())
            using (var secondEnumerator = second.GetEnumerator())
            {
                bool firstCond = firstEnumerator.MoveNext();
                bool secondCond = secondEnumerator.MoveNext();

                while (firstCond && secondCond)
                {
                    if (predicate(firstEnumerator.Current, secondEnumerator.Current))
                    {
                        yield return firstEnumerator.Current;
                        firstCond = firstEnumerator.MoveNext();
                    }
                    else
                    {
                        yield return secondEnumerator.Current;
                        secondCond = secondEnumerator.MoveNext();
                    }
                }

                while (firstCond)
                {
                    yield return firstEnumerator.Current;
                    firstCond = firstEnumerator.MoveNext();
                }

                while (secondCond)
                {
                    yield return secondEnumerator.Current;
                    secondCond = secondEnumerator.MoveNext();
                }
            }
        }

        public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            foreach (var item in collection)
                oc.Add(item);
        }
    }
}