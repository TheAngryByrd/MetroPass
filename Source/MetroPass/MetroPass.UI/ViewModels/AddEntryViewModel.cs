using System.Xml.Linq;
using Caliburn.Micro;
using System;
using System.Linq;
using Framework;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;
using MetroPass.UI.Views;

namespace MetroPass.UI.ViewModels
{
    public class AddEntryViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IKdbTree _dbTree;

        public AddEntryViewModel(IKdbTree dbTree, INavigationService navigationService) : base(navigationService)
        {
            _dbTree = dbTree;
            _navigationService = navigationService;
        }

        private string _parentGroupID;
        public string ParentGroupID
        {
            get { return _parentGroupID; }
            set
            {
                _parentGroupID = value;
                var groupElement = _dbTree.FindGroupByUuid(value);
                ParentGroup = new PwGroup(groupElement);
            }
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
                    <UUID>{0}</UUID>
                    <IconID>0</IconID>
                    <Times>
                        <LastModificationTime>{1}</LastModificationTime>
                        <CreationTime>{1}</CreationTime>
                        <LastAccessTime>{1}</LastAccessTime>
                        <ExpiryTime>{1}</ExpiryTime>
                        <LocationChanged>{1}</LocationChanged>
                        <Expires>False</Expires>
                        <UsageCount>0</UsageCount>
                    </Times>
                    <String>
                        <Key>Title</Key>
                        <Value>{2}</Value>
                    </String>
                    <String>
                        <Key>UserName</Key>
                        <Value>{3}</Value>
                    </String>
                    <String>
                        <Key>Password</Key>
                        <Value Protected=""True"">{4}</Value>
                    </String>
                    <String>
                        <Key>URL</Key>
                        <Value>{5}</Value>
                    </String>
                    <String>
                        <Key>Notes</Key>
                        <Value>{6}</Value>
                    </String>
                </Entry>
            ";
            var uuid = new PwUuid(true);
            entryTemplate = String.Format(entryTemplate, Convert.ToBase64String(uuid.UuidBytes), DateTime.Now.ToFormattedUtcTime(), Title, Username, Password, Url, Notes);

            var element = XElement.Parse(entryTemplate);
            return element;
        }
    }
}