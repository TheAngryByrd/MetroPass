using Caliburn.Micro;
using Framework;
using MetroPass.UI.Views;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MetroPass.UI.ViewModels
{
    public class NewDatabaseViewModel : BaseScreen 
    {
        private readonly INavigationService _navigationService;

        public NewDatabaseViewModel( INavigationService navigationService)
            : base(navigationService)
        {
                _navigationService = navigationService;
        }

        private string _name;
        public string Name
        {
            get { return _name; }

            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
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
            var databaseXDocument = NewDatabase();


            _navigationService.NavigateToViewModel<EntryGroupListViewModel>();
        }

        public XDocument NewDatabase()
        {
            var databaseXML = @"<KeePassFile>
                                  <Meta>
                                    <Generator>{0}</Generator>
                                    <HeaderHash>0GuRCYFy2gzb1mm508FVJ1I0wDkJFHN3g0oN0GvSZ04=</HeaderHash>
                                    <DatabaseName>{1}</DatabaseName>
                                    <DatabaseNameChanged>{2}</DatabaseNameChanged>
                                    <DatabaseDescription />
                                    <DatabaseDescriptionChanged>{2}</DatabaseDescriptionChanged>
                                    <DefaultUserName />
                                    <DefaultUserNameChanged>{2}</DefaultUserNameChanged>
                                    <MaintenanceHistoryDays>365</MaintenanceHistoryDays>
                                    <Color />
                                    <MasterKeyChanged>{2}</MasterKeyChanged>
                                    <MasterKeyChangeRec>-1</MasterKeyChangeRec>
                                    <MasterKeyChangeForce>-1</MasterKeyChangeForce>
                                    <MemoryProtection>
                                      <ProtectTitle>False</ProtectTitle>
                                      <ProtectUserName>False</ProtectUserName>
                                      <ProtectPassword>True</ProtectPassword>
                                      <ProtectURL>False</ProtectURL>
                                      <ProtectNotes>False</ProtectNotes>
                                    </MemoryProtection>
                                    <RecycleBinEnabled>True</RecycleBinEnabled>
                                    <RecycleBinUUID>AAAAAAAAAAAAAAAAAAAAAA==</RecycleBinUUID>
                                    <RecycleBinChanged>{2}</RecycleBinChanged>
                                    <EntryTemplatesGroup>AAAAAAAAAAAAAAAAAAAAAA==</EntryTemplatesGroup>
                                    <EntryTemplatesGroupChanged>{2}</EntryTemplatesGroupChanged>
                                    <HistoryMaxItems>10</HistoryMaxItems>
                                    <HistoryMaxSize>6291456</HistoryMaxSize>
                                    <LastSelectedGroup></LastSelectedGroup>
                                    <LastTopVisibleGroup></LastTopVisibleGroup>
                                    <Binaries />
                                    <CustomData />
                                  </Meta>
                                  <Root>
  
                                    <DeletedObjects />
                                  </Root>
                                </KeePassFile>";
           
            var formatedXml = string.Format(databaseXML, "MetroPass", Name, DateTime.Now.ToFormattedUtcTime());
            return XDocument.Parse(formatedXml);
        }
    }
}
