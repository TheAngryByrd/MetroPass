﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace Metropass.Core.PCL.Model
{
    public class PwGroup : PwCommon, IGroup
    {
        private readonly ObservableCollection<PwEntry> _entries;
        private readonly ObservableCollection<PwGroup> _subGroups;

        public PwGroup(XElement element) : this(element, true) { }

        public PwGroup(XElement element, bool includeSubGroups) 
        {
            Element = element;
            if (element != null)
            {
                _entries = new ObservableCollection<PwEntry>(from entry in element.Elements("Entry") select new PwEntry(entry, this));

                if (includeSubGroups)
                {
                    _subGroups = new ObservableCollection<PwGroup>(from @group in element.Elements("Group") select new PwGroup(@group));
                }
                else
                {
                    _subGroups = new ObservableCollection<PwGroup>();
                }
                
                _subGroupsAndEntries = new Lazy<ObservableCollection<PwCommon>>(() => new ObservableCollection<Metropass.Core.PCL.Model.PwCommon>(this.SubGroups.Union(this.Entries.Cast<PwCommon>())));
            }
    
        }

        public string Name 
        { 
            get
            {
                return Element.Element().Value;
            }
            set
            {
                Element.Element().Value = value;
            }
        }

        public IEnumerable<PwEntry> Entries
        {
            get {
                return _entries.OrderBy(a => a.Title);
            }
        }

        public int EntryCount
        {
            get { return _entries.Count(); }
        }


        public IEnumerable<PwGroup> SubGroups
        {
            get {
               return _subGroups;
            }
        }  
     

        public int SubGroupCount
        {
            get { return _subGroups.Count(); }
        }

        private readonly Lazy<ObservableCollection<PwCommon>> _subGroupsAndEntries;

        public ObservableCollection<PwCommon> SubGroupsAndEntries
        {
            get
            {
                return _subGroupsAndEntries.Value;
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
            _entries.Add(entry);
        }

        public void AddGroupToDocument(PwGroup group)
        {
            Element.Add(group.Element);
            _subGroups.Add(group);
        }

        public static PwGroup NullGroup
        {
            get
            {
                var emptyGroup = XElement.Parse(@"
                <Group>
                    <UUID></UUID>
                    <Name></Name>
                </Group>");

                return new PwGroup(emptyGroup, false);
            }
        }
    }
}