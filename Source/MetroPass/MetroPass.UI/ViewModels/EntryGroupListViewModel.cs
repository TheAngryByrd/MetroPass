using System.Collections.Generic;
using Caliburn.Micro;
using MetroPass.Core.Model;
using Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using MetroPass.UI.Services;

namespace MetroPass.UI.ViewModels
{
    public class EntryGroupListViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<PwGroup> _entryGroupsWithEntries;
        private readonly IClipboard _clipboard;

        public EntryGroupListViewModel(INavigationService navigationService, IClipboard clipboard) : base(navigationService)
        {
            _clipboard = clipboard;
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

        private PwCommon _selectedPasswordItem;
        public PwCommon SelectedPasswordItem
        {
            get { return _selectedPasswordItem; }
            set
            {
                _selectedPasswordItem=value;
                if (value is PwGroup)
                {
                    ShowAppBar = false;
                    _navigationService.NavigateToViewModel<EntryGroupListViewModel, PwGroup>((PwGroup)value, vm => vm.Root);
                }
                else if (value != null)
                {
                    ShowAppBar = true;
                }
                NotifyOfPropertyChange(() => SelectedPasswordItem);
                NotifyOfPropertyChange(() => ShowEntryCommands);
            }
        }

        public  void EditGroup()
        {
            _navigationService.NavigateToViewModel<GroupEditViewModel, PwGroup>(Root, vm => vm.Group);
        }

        private bool _showAppBar;
        public bool ShowAppBar
        {
            get { return _showAppBar; }
            set
            {
                _showAppBar = value;
                NotifyOfPropertyChange(() => ShowAppBar);
            }
        }

        public void DeselectItem()
        {
            SelectedPasswordItem = null;
        }

        public bool ShowEntryCommands
        {
            get { return _selectedPasswordItem is PwEntry; }
        }

        public async void CopyUsername()
        {
            var entry = _selectedPasswordItem as PwEntry;
            if (entry != null)
            {
                await _clipboard.CopyToClipboard(entry.Username);
            }
        }

        public async void CopyPassword()
        {
            var entry = _selectedPasswordItem as PwEntry;
            if (entry != null)
            {
                await _clipboard.CopyToClipboard(entry.Password);
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

        public IEnumerable<PwGroup> GroupsOnThisLevel
        {
            get { return Root.SubGroups; }
        }

        public void SelectGroup(PwGroup selectedGroup)
        {
            _navigationService.NavigateToViewModel<EntryGroupListViewModel, PwGroup>(selectedGroup, vm => vm.Root);
        }
    }
}