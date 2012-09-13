using Caliburn.Micro;
using MetroPass.Core.Model;
using Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MetroPass.UI.ViewModels
{
    public class EntryGroupListViewModel : BaseScreen
    {
        private readonly INavigationService navigationService;
        private readonly ObservableCollection<PwGroup> _entryGroupsWithEntries;

        public EntryGroupListViewModel(INavigationService navigationService): base(navigationService)
        {
            _entryGroupsWithEntries = new ObservableCollection<PwGroup>();
        }

        PwGroup _root = null;
        public PwGroup Root
        {
            get { return _root; }
            set
            {
                _root = value;
                _entryGroupsWithEntries.AddRange(value.SubGroups);
                NotifyOfPropertyChange(() => Root);
                NotifyOfPropertyChange(() => EntryGroupsWithEntries);
            }
        }

        public ObservableCollection<PwGroup> EntryGroupsWithEntries
        {
            get
            {
                //var temp = new ObservableCollection<PwGroup>();

                //temp.AddRange(Root.SubGroups);
                //temp.Add(new PwGroup(null, this.Root.Entries) { Name = "Entries"});

                //return temp;
                return _entryGroupsWithEntries;
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
