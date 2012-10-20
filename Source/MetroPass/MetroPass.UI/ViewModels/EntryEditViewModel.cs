using Caliburn.Micro;
using System;
using System.Linq;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;
using System.Diagnostics;

namespace MetroPass.UI.ViewModels
{
    public class EntryEditViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;

        public EntryEditViewModel(INavigationService navigationService) : base(navigationService)
        {
            this._navigationService=navigationService;
        }

        private PwEntry _pwEntry;
        public PwEntry Entry
        {
        	get { return _pwEntry; }
            set
            {
                _pwEntry = value;
                Title = _pwEntry.Title;
                Username = _pwEntry.Username;
                Password = _pwEntry.Password;
                Confirm = _pwEntry.Password;
                Url = _pwEntry.Url;
                Notes = _pwEntry.Notes;
                NotifyOfPropertyChange(() => Entry);
            }
        }

        private string _title;
        public string Title
        {
        	get	{ return _title; }

            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        private string _userName;
        public string Username
        {
            get { return _userName; }

            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }

            set
            {
                _password = value;
                CanSave = value == Confirm;
                NotifyOfPropertyChange(() => Password);
            }
        }

        private string _confirm;
        public string Confirm
        {
            get { return _confirm; }
            set
            {
                _confirm = value;
                CanSave = value == Password;
                NotifyOfPropertyChange(() => Password);
            }
        }

        private string _url;
        public string Url
        {
        	get { return _url; }
            set
            {
                _url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        private string _notes;
        public string Notes
        {
        	get { return _notes; }
            set
            {
                _notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        
        }

        private bool _canSave = true;
        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                _canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public async void Save()
        {
            CanSave = false;
            Entry.Title = Title;
            Entry.Username = Username;
            Entry.Password = Password;
            Entry.Url = Url;
            Entry.Notes = Notes;
            await PWDatabaseDataSource.Instance.SavePwDatabase();
            _navigationService.GoBack();
        }
    }
}