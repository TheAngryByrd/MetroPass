using System;
using System.Collections.Generic;

namespace Metropass.Core.PCL.Model.Kdb4
{
    public static class ColletionExtensions
    {
        public static T FindAll<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren, Func<T, bool> predicate)
        {
            if (null == source)
                throw new ArgumentNullException("source");
            if (null == getChildren)
                throw new ArgumentNullException("getChildren");
            return FindAllIterator(source, getChildren, predicate);
        }

        private static T FindAllIterator<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren, Func<T, bool> predicate)
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
                        if (predicate(current))
                            return current;

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
                {
                    stack.Pop().Dispose();
                }
            }

            return default(T);
        }

        public static IEnumerable<T> All<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren)
        {
            if (null == source)
                throw new ArgumentNullException("source");
            if (null == getChildren)
                return source;
            return AllIterator(source, getChildren);
        }

        private static IEnumerable<T> AllIterator<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren)
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
                {
                    stack.Pop().Dispose();
                }
            }
        }
    }
}