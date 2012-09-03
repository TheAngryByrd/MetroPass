using Framework;
using MetroPass.Core.Model;
using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.UI.ViewModels
{
    public class EntryGroupListPageViewModel : BindableBase
    {
        PwGroup _root = null;
        public PwGroup Root
        {
            get
            {

                return _root;
            }
            set
            {
                SetProperty(ref _root, value);
            }
        }

        public EntryGroupListPageViewModel(PwGroup entryGroup)
        {
            // TODO: Complete member initialization
            this.Root = entryGroup;
        }

        public ObservableCollection<PwGroup> EntryGroupsWithEntries
        {
            get
            {
                var temp = new ObservableCollection<PwGroup>();

                temp.AddRange(Root.SubGroups);
                temp.Add(new PwGroup(null) { Name = "Entries", Entries = this.Root.Entries });

                return temp;
            }
        }

        public ObservableCollection<object> AllTogetherNow
        {
            get
            {
                var temp = new ObservableCollection<object>();

                temp.AddRange(Root.SubGroups);
                temp.AddRange(Root.Entries);

                return temp;
            }
        }
    }
}
