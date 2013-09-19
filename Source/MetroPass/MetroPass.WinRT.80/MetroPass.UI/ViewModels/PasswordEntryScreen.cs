using System;
using System.Net;
using Caliburn.Micro;
using MetroPass.UI.Services;
using Metropass.Core.PCL.Model;

namespace MetroPass.UI.ViewModels
{
    public class PasswordEntryScreen : BaseScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IClipboard _clipboard;

        public PasswordEntryScreen(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IClipboard clipboard,
            IPageServices pageServices) : base(navigationService, eventAggregator, pageServices)
        {
            _navigationService = navigationService;
            _clipboard = clipboard;
        }

        protected PwCommon _selectedPasswordItem;
        public PwCommon SelectedPasswordItem
        {
            get { return _selectedPasswordItem; }
            set
            {
                _selectedPasswordItem = value;
                if (value is PwGroup)
                {
                    ShowAppBar = false;
                    var encodedUUID =value.UUID;
                    _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
                }
                else if (value != null)
                {
                    ShowAppBar = true;
                }
                NotifyOfPropertyChange(() => SelectedPasswordItem);
                NotifyOfPropertyChange(() => ShowEntryCommands);
                NotifyOfPropertyChange(() => ShowGroupCommands);
                NotifyOfPropertyChange(() => CanOpenURL);
            }
        }

        protected bool _showAppBar;
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

        public bool ShowGroupCommands
        {
            get { return _selectedPasswordItem == null; }
        }

        public void EditEntry()
        {
            var encodedUUID =SelectedPasswordItem.UUID;
            _navigationService.UriFor<EntryEditViewModel>().WithParam(vm => vm.EntryID, encodedUUID).Navigate();
        }

        public void OpenURL()
        {
            var password = SelectedPasswordItem as PwEntry;
            var uri = GetPasswordUri(password);
            LaunchUrl(uri);
        }

        public bool CanOpenURL
        {
            get
            {
                var password = SelectedPasswordItem as PwEntry;
                if (password != null)
                {
                    return !String.IsNullOrWhiteSpace(password.Url);
                }
                return false;
            }
        }

        public void DeselectItem()
        {
            SelectedPasswordItem = null;
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
    }
}
