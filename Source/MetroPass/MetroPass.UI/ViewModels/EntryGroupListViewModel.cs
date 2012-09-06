using Caliburn.Micro;
using MetroPass.Core.Model;
using Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MetroPass.UI.ViewModels
{
    public class EntryGroupListViewModel : Screen
    {
        private readonly INavigationService navigationService;

        public EntryGroupListViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public PwGroup Parameter
        {
            set { this.Root = value; }
        }

        PwGroup _root = null;
        public PwGroup Root
        {
            get { return _root; }
            set
            {
                _root = value;
                NotifyOfPropertyChange(() => Root);
            }
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
