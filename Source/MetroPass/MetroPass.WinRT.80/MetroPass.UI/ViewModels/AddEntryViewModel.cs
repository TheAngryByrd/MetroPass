﻿using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.ViewModels.Messages;
using MetroPass.UI.Views;
using System;
using System.Net;
using System.Xml.Linq;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;
using Windows.UI.Xaml.Media;

namespace MetroPass.UI.ViewModels
{
    public class AddEntryViewModel : BaseScreen, IHandle<PasswordGenerateMessage>
    {
        private readonly INavigationService _navigationService;


        private readonly IPWDatabaseDataSource _dataSource;

        public AddEntryViewModel(
            INavigationService navigationService,
            IPageServices pageServices,
            IEventAggregator eventAggregator,
            IPWDatabaseDataSource dataSource)
            : base(navigationService, eventAggregator, pageServices)
        {
            _dataSource = dataSource;
            _navigationService = navigationService;
        }

        private string _parentGroupID;
        public string ParentGroupID
        {
            get { return _parentGroupID; }
            set
            {
                _parentGroupID = value;
                ParentGroup = _dataSource.PwDatabase.Tree.FindGroupByUuid(value);               
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

        public string Sanitize(string text)
        {
            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(text));
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
                
            var view = this.View as IPasswordErrorStateView;
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
            var entry = new PwEntry(entryElement, ParentGroup);
            ParentGroup.AddEntryToDocument(entry);
            await _dataSource.SavePwDatabase();
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

            entryTemplate = String.Format(entryTemplate, Convert.ToBase64String(uuid.UuidBytes), DateTime.Now.ToFormattedUtcTime(), Sanitize(Title), Sanitize(Username), Sanitize(Password) ,Sanitize(Url) , Sanitize(Notes));

            var element = XElement.Parse(entryTemplate);
            return element;
        }

        public void Generate()
        {
            var settingsColor = App.Current.Resources["MainAppColor"] as SolidColorBrush;
            DialogService.ShowSettingsFlyout<PasswordGeneratorViewModel>(this, headerBrush: settingsColor);
        }

        public void Handle(PasswordGenerateMessage message)
        {
            Password = message.GeneratedPassword;
            Confirm = message.GeneratedPassword;
        }

    }
}