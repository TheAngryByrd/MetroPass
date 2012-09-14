using Caliburn.Micro;
using MetroPass.Core.Model;
using Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using MetroPass.UI.Services;
using Windows.ApplicationModel.DataTransfer;

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
                else
                {
                    ShowAppBar = true;
                }
                NotifyOfPropertyChange(() => ShowEntryCommands);
            }
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

        public bool ShowEntryCommands
        {
            get { return _selectedPasswordItem is PwEntry; }
        }

        public async void CopyUsername()
        {
            var entry = _selectedPasswordItem as PwEntry;
            if (entry != null)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(entry.Username);
                await _clipboard.CopyToClipboard(dataPackage);
            }
        }

        public async void CopyPassword()
        {
            var entry = _selectedPasswordItem as PwEntry;
            if (entry != null)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(entry.Password);
                await _clipboard.CopyToClipboard(dataPackage);
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