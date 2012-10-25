using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public class PwGroup : PwCommon
    {    
        private readonly IEnumerable<XElement> _entries;
        private readonly IEnumerable<XElement> _subGroups;

        public string Name 
        { 
            get { return Element.Element().Value; } 
            set { Element.Element().Value = value; }
        }

        public IEnumerable<PwEntry> Entries
        {
            get { 
                return _entries.Select(e => new PwEntry(e)).OrderBy(e => e.Title);
            }
        }

        public int EntryCount
        {
            get { return _entries.Count(); }
        }

        public IEnumerable<PwGroup> SubGroups
        {
            get { 
                return _subGroups.Select(e => new PwGroup(e)).OrderBy(g => g.Name);
            }
        }

        public int SubGroupCount
        {
            get { return _subGroups.Count(); }
        }

        public IEnumerable<PwCommon> AllTogetherNow
        {
            get
            {
                return this.SubGroups.Union(this.Entries.Cast<PwCommon>());
            }
        }

        public void AddEntryToDocument(PwEntry entry)
        {
            var lastEntry = Element.Elements("Entry").LastOrDefault();
            if (lastEntry != null)
            {
                lastEntry.AddAfterSelf(entry.Element);
            }
            else
            {
                Element.Add(entry.Element);
            }
        }

        public PwGroup(XElement element)
        {
            Element = element;
            _entries = element.Elements("Entry");
            _subGroups = element.Elements("Group");
        }
    }
}
