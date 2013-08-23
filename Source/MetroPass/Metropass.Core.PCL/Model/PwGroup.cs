using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Metropass.Core.PCL.Model
{
    public class PwGroup : PwCommon, IGroup
    {
        private ObservableCollection<PwEntry> _entries
        {
            get
            {
                return _entriesLazy.Value;
            }
        }
        private ObservableCollection<PwGroup> _subGroups
        {
            get
            {
                return _subGroupsLazy.Value;
            }
        }
        private readonly Lazy<ObservableCollection<PwEntry>> _entriesLazy;
        private readonly Lazy<ObservableCollection<PwGroup>> _subGroupsLazy;        
        
        //private readonly ObservableCollection<PwEntry> _entries;
        //private readonly ObservableCollection<PwGroup> _subGroups;

        public PwGroup(XElement element) : this(element, true) { }

        public PwGroup(XElement element, bool includeSubGroups) 
        {
            Element = element;
            if (element != null)
            {
                _entriesLazy = new Lazy<ObservableCollection<PwEntry>>(() => IndexEntries(element));
               
                if (includeSubGroups)
                {
                    _subGroupsLazy = new Lazy<ObservableCollection<PwGroup>>(() =>IndexGroups(element));
                }
                else
                {
                    _subGroupsLazy = new Lazy<ObservableCollection<PwGroup>>(() => new ObservableCollection<PwGroup>());
                }

                _subGroupsAndEntries = new AsyncLazy<ObservableCollection<PwCommon>>(() => GetSubGroupsAndEntries());
            }
    
        }

        private ObservableCollection<PwEntry> IndexEntries(XElement element)
        {
            return new ObservableCollection<PwEntry>(element.Elements("Entry").Select(entry => new PwEntry(entry, this)));
        }

        private ObservableCollection<PwGroup> IndexGroups(XElement element)
        {
            return new ObservableCollection<PwGroup>(element.Elements("Group").Select(@group => new PwGroup(@group)));
        }

        private async Task<ObservableCollection<PwCommon>> GetSubGroupsAndEntries()
        {
            var x = new List<PwCommon>();

             
            var t1 = Task.Run(() => x.AddRange(this.SubGroups));
            var t2 = Task.Run(() => x.AddRange(Entries));
            await Task.WhenAll(t1, t2);
            //x.AddRange(Entries);
            return new ObservableCollection<PwCommon>(x);          
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

        private readonly AsyncLazy<ObservableCollection<PwCommon>> _subGroupsAndEntries;

        public ObservableCollection<PwCommon> SubGroupsAndEntries
        {
            get
            {
                return _subGroupsAndEntries.Value.Result;
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