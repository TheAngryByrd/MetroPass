using System;
using System.Collections.Generic;
using Metropass.Core.PCL.Model;

namespace MetroPass.UI.ViewModels
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<PwEntry> AllEntries(this PwGroup node)
        {
            //yield return node;
            foreach (var entry in node.Entries) {
                yield return entry;
            }

            foreach (var childNode in node.SubGroups)
                foreach (var allItem in AllEntries(childNode))
                    yield return allItem;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}