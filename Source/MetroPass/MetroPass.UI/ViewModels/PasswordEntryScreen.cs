using System;
using System.Linq;
using System.Net;
using Caliburn.Micro;
using MetroPass.Core.Model;
using MetroPass.UI.Services;

namespace MetroPass.UI.ViewModels
{
    public class PasswordEntryScreen : BaseScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IClipboard _clipboard;

        public PasswordEntryScreen(INavigationService navigationService, IClipboard clipboard, IPageServices pageServices) : base(navigationService, pageServices)
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
                    var encodedUUID = WebUtility.UrlEncode(value.UUID);
                    _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
                }
                else if (value != null)
                {
                    ShowAppBar = true;
                }
                NotifyOfPropertyChange(() => SelectedPasswordItem);
                NotifyOfPropertyChange(() => ShowEntryCommands);
                NotifyOfPropertyChange(() => ShowGroupCommands);
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
            var encodedUUID = WebUtility.UrlEncode(SelectedPasswordItem.UUID);
            _navigationService.UriFor<EntryEditViewModel>().WithParam(vm => vm.EntryID, encodedUUID).Navigate();
        }

        public void OpenURL()
        {
            this.LaunchUrl((SelectedPasswordItem as PwEntry).Url);
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
