using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Metropass.Core.PCL.Exceptions;

namespace Metropass.Core.PCL.Model.Kdb4
{
    public class Kdb4Tree : IKdbTree
    {
        public XDocument Document { get;  set; }
        public PwGroup Group { get; set; }
        public Kdb4Tree(XDocument document)
        {
            Document = document;
            MetaData = new Kdb4TreeMetaData(document);
        }

        public Kdb4TreeMetaData MetaData { get; set; }

        public PwEntry FindEntryByUuid(string entryId)
        {
            var source = new List<PwGroup>() { Group };
            var result = source.All(g => g.SubGroups).
                SelectMany(g => g.Entries).
                SingleOrDefault(e => e.UUID == entryId);

            if (result != null)
            {
                return result;
            }           
          
            throw new ArgumentException(string.Format("Could not find Entry with ID {0} in the database.", entryId), entryId);
        }

        public PwGroup FindGroupByUuid(string groupId)
        {
            var source = new List<PwGroup>() { Group };
            var result = source.FindAll(g => g.SubGroups, g => g.UUID == groupId);

            if (result != null)
            {
                return result;
            }
            throw new GroupNotFoundException(string.Format("Cound not find Group with ID {0} in the database.", groupId), groupId);
        }
    }

    public static class ColletionExtensions
    {
        public static T FindAll<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildren, Func<T, bool> predicate)
        {
            if (null == source) throw new ArgumentNullException("source");
            if (null == getChildren) throw new ArgumentNullException("getChildren");
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
                        if(predicate(current))
                            return current;

                        var children = getChildren(current);
                        if (null != children) stack.Push(children.GetEnumerator());
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
            if (null == source) throw new ArgumentNullException("source");
            if (null == getChildren) return source;
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
                        if (null != children) stack.Push(children.GetEnumerator());
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