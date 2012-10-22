using System.Xml.Linq;
using Caliburn.Micro;
using System;
using System.Linq;
using Framework;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;
using MetroPass.UI.Views;

namespace MetroPass.UI.ViewModels
{
    public class AddEntryViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;

        public AddEntryViewModel(INavigationService navigationService) : base(navigationService)
        {
            this._navigationService=navigationService;
        }

        private PwGroup _parentGroup;
        public PwGroup ParentGroup
        {
        	get { return _parentGroup; }
            set
            {
                _parentGroup = value;
                NotifyOfPropertyChange(() => ParentGroup);
            }
        }

        public string NewEntryTitle
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    return "New Password";
                }
                return String.Format("New Password ({0})", Title);
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
                NotifyOfPropertyChange(() => NewEntryTitle);
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
                ConfirmPassword();
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
                ConfirmPassword();
                NotifyOfPropertyChange(() => Confirm);
            }
        }

        private void ConfirmPassword()
        {
            bool passwordsMatch = Password == Confirm;
            CanSave = passwordsMatch;
                
            var view = this.View as IEntryEditView;
            if (view != null)
            {
                view.SetPasswordState(passwordsMatch);
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

        private bool _canSave = false;
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
            var entryElement = GetNewEntryElement();
            var entry = new PwEntry(entryElement);
            ParentGroup.AddEntryToDocument(entry);
            await PWDatabaseDataSource.Instance.SavePwDatabase();
            _navigationService.GoBack();
        }

        private XElement GetNewEntryElement()
        {
            var entryTemplate = @"
                <Entry>
                    <IconID>0</IconID>
                    <Times>
                        <LastModificationTime>{0}</LastModificationTime>
                        <CreationTime>{0}</CreationTime>
                        <LastAccessTime>{0}</LastAccessTime>
                        <ExpiryTime>{0}</ExpiryTime>
                        <LocationChanged>{0}</LocationChanged>
                        <Expires>False</Expires>
                        <UsageCount>0</UsageCount>
                    </Times>
                    <String>
                        <Key>Title</Key>
                        <Value>{1}</Value>
                    </String>
                    <String>
                        <Key>UserName</Key>
                        <Value>{2}</Value>
                    </String>
                    <String>
                        <Key>Password</Key>
                        <Value Protected=""True"">{3}</Value>
                    </String>
                    <String>
                        <Key>URL</Key>
                        <Value>{4}</Value>
                    </String>
                    <String>
                        <Key>Notes</Key>
                        <Value>{5}</Value>
                    </String>
                </Entry>
            ";
            entryTemplate = String.Format(entryTemplate, DateTime.Now.ToFormattedUtcTime(), Title, Username, Password, Url, Notes);

            var element = XElement.Parse(entryTemplate);
            return element;
        }
    }
}