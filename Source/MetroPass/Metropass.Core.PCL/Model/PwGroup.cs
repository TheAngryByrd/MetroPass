using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace Metropass.Core.PCL.Model
{
    public class PwGroup : Metropass.Core.PCL.Model.PwCommon, Metropass.Core.PCL.Model.IGroup
    { 
        private readonly IEnumerable<XElement> _entries;
        private readonly IEnumerable<XElement> _subGroups;

        public PwGroup(XElement element) : this(element, true) { }

        public PwGroup(XElement element, bool includeSubGroups) 
        {
            Element = element;
            if (element != null)
            {
                _entries = element.Elements("Entry");

                if (includeSubGroups)
                {
                    _subGroups = element.Elements("Group");
                }
                else
                {
                    _subGroups = new XElement[0];
                }
                _subGroupsAndEntries = new ObservableCollection<Metropass.Core.PCL.Model.PwCommon>(this.SubGroups.Union(this.Entries.Cast<PwCommon>()));
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
                return _entries.Select(e => new PwEntry(e, this)).OrderBy(e => e.Title);
            }
        }

        public int EntryCount
        {
            get { return _entries.Count(); }
        }

        public IEnumerable<PwGroup> SubGroups
        {
            get {
                int i = 0, subGroupCount = _subGroups.Count();
                return _subGroups
                    .OrderBy(e => e.Element("Name").Value)
                    .Select(e =>
                        new
                        {
                            Group = new PwGroup(e),
                            SortKey = e.Element("Name").Value == "Backup" ? ++subGroupCount : e.Element("Name").Value == "Recycle Bin" ? ++subGroupCount : ++i
                        })
                    .OrderBy(g => g.SortKey)
                    .Select(g => g.Group);
            }
        }

        public int SubGroupCount
        {
            get { return _subGroups.Count(); }
        }

        private readonly ObservableCollection<Metropass.Core.PCL.Model.PwCommon> _subGroupsAndEntries;

        public ObservableCollection<Metropass.Core.PCL.Model.PwCommon> SubGroupsAndEntries
        {
            get
            {
                return _subGroupsAndEntries;
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

        public void AddGroupToDocument(PwGroup group)
        {
            Element.Add(group.Element);
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