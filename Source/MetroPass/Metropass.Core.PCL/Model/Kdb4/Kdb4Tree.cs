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

        public XElement FindEntryByUuid(string entryId)
        {
            var entryElements = Document.Descendants("Entry").Where(e => e.Element("UUID") != null && e.Element("UUID").Value == entryId);
            if (entryElements.Count() == 1)
            {
                return entryElements.Single();
            }
            if (entryElements.Count() > 0)
            {
                entryElements = entryElements.Where(e => e.Parent.Name != "History");
                if (entryElements.Count() == 1)
                {
                    return entryElements.Single();
                }
            }
            throw new ArgumentException(string.Format("Could not find Entry with ID {0} in the database.", entryId), entryId);
        }

        public PwGroup FindGroupByUuid(string groupId)
        {
            var result = new List<PwGroup>() { Group };
            result = result.All(g => g.SubGroups).Where(g => g.UUID == groupId).ToList();

            if (result.Any())
            {
                return result.Single();
            }
            throw new GroupNotFoundException(string.Format("Cound not find Group with ID {0} in the database.", groupId), groupId);
        }
    }

    public static class Exte
    {
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