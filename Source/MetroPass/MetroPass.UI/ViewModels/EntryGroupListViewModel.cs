using System.Xml.Linq;
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
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<PwGroup> _entryGroupsWithEntries;

        private PwCommon _selectedPasswordItem;

        public EntryGroupListViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            _entryGroupsWithEntries = new ObservableCollection<PwGroup>();
        }

        PwGroup _root = null;
        public PwGroup Root
        {
            get { return _root; }
            set
            {
                _root = value;
                _entryGroupsWithEntries.Add(new PwGroup(value.Element, value.Entries));
                _entryGroupsWithEntries.AddRange(value.SubGroups);
                NotifyOfPropertyChange(() => Root);
            }
        }

       public PwCommon SelectedPasswordItem
        {
            get { return _selectedPasswordItem; }
            set
            {
                _selectedPasswordItem=value;
                if (value is PwGroup)
                {
                    _navigationService.NavigateToViewModel<EntryGroupListViewModel, PwGroup>((PwGroup)value, vm => vm.Root);
                }
            }
        }

        public ObservableCollection<PwGroup> EntryGroupsWithEntries
        {
            get { return _entryGroupsWithEntries; }
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