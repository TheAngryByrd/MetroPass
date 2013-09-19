using System.Linq;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.ViewModels.Messages;
using MetroPass.UI.Views;
using MetroPass.UI.Services;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;
using Windows.UI.Xaml.Media;

namespace MetroPass.UI.ViewModels
{
    public class EntryEditViewModel : BaseScreen, IHandle<PasswordGenerateMessage>
    {
        private readonly INavigationService _navigationService;
        private bool _loadingData = true;

        private readonly IPWDatabaseDataSource _dataSource;

        public EntryEditViewModel(
            INavigationService navigationService,
            IPageServices pageServices,
            IEventAggregator eventAggregator,
            IPWDatabaseDataSource dataSource) :
            base(navigationService, eventAggregator, pageServices)
        {
            _dataSource = dataSource;
            _navigationService = navigationService;
        }

        private string _entryID;
        public string EntryID
        {
            get { return _entryID; }
            set
            {
                _entryID = value;
                var entryElement = _dataSource.PwDatabase.Tree.FindEntryByUuid(value);
                Entry = entryElement;
            }
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
                _loadingData = false;
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

        public string MaskedPassword
        {
            get
            {
                return string.Concat(Password.ToCharArray().Select(x => '*'));
            }
        }

        private string _password;
  
        public string Password
        {
            get { return _password; }

            set
            {
                _password = value;
                ConfirmPassword();
                NotifyOfPropertyChange(() => Password);
            }
        }

        public string MaskedConfirm
        {
            get
            {
                return string.Concat(Confirm.ToCharArray().Select(x => '*'));
            }
        }

        private string _confirm;

        public string Confirm
        {
            get { return _confirm; }
            set
            {
                _confirm = value;
                ConfirmPassword();
                NotifyOfPropertyChange(() => Confirm);
            }
        }

        private void ConfirmPassword()
        {
            if (!_loadingData)
            {
                bool passwordsMatch = Password == Confirm;
                CanSave=passwordsMatch;
                
                var view = this.View as IPasswordErrorStateView;
                if (view != null)
                {
                    view.SetPasswordState(passwordsMatch);
                }
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

        private bool isProgressEnabled;

        public bool IsProgressEnabled
        {
            get { return isProgressEnabled; }
            set { isProgressEnabled = value;
            NotifyOfPropertyChange(() => IsProgressEnabled);
            }
        }

        private bool canGoBack = true;

        public override bool CanGoBack
        {
            get
            {
                return base.CanGoBack && canGoBack;
            }
            set
            {
                canGoBack = value;
                NotifyOfPropertyChange(() => CanGoBack);
            }
        }

        public void OpenUrl()
        {
            var uri = GetPasswordUri(Entry);
            LaunchUrl(uri);
        }

        public void Generate()
        {
            var settingsColor = App.Current.Resources["MainAppColor"] as SolidColorBrush;
            DialogService.ShowSettingsFlyout<PasswordGeneratorViewModel>(this,headerBrush: settingsColor);
        }

        public async void Save()
        {
            CanGoBack = false;
            IsProgressEnabled = true;
            CanSave = false;
            Entry.Title = Title;
            Entry.Username = Username;
            Entry.Password = Password;
            Entry.Url = Url;
            Entry.Notes = Notes;
            await _dataSource.SavePwDatabase();
            _navigationService.GoBack();
        }

        public void Handle(PasswordGenerateMessage message)
        {
            Password = message.GeneratedPassword;
            Confirm = message.GeneratedPassword;
        }
    }
}
