using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public class PwGroup : PwCommon
    {    
        private readonly List<PwEntry> _entries;
        private readonly List<PwGroup> _subGroups;

        public string Name 
        { 
            get { return Element.Element().Value; } 
            set { Element.Element().Value = value; }
        }

        public IEnumerable<PwEntry> Entries
        {
            get { return _entries; }
        }

        public int EntryCount
        {
            get { return _entries.Count(); }
        }

        public IEnumerable<PwGroup> SubGroups
        {
            get { return this._subGroups; }
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

        public void AddEntry(PwEntry entry)
        {
            _entries.Add(entry);
        }

        public void AddEntryToDocument(PwEntry entry)
        {
            this.AddEntry(entry);
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

        public void AddSubGroup(PwGroup subGroup)
        {
            _subGroups.Add(subGroup);
        }

        public PwGroup(XElement element)
        {
            Element = element;
            _entries = new List<PwEntry>();
            _subGroups = new List<PwGroup>();
        }

        public PwGroup(XElement element, IEnumerable<PwEntry> entries)
        {
            Element = element;
            _entries = new List<PwEntry>(entries);
            _subGroups = new List<PwGroup>();
        }
    }
}
