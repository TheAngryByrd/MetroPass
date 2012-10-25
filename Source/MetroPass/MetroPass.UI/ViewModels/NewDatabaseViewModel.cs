using Caliburn.Micro;
using MetroPass.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetroPass.UI.ViewModels
{
    public class NewDatabaseViewModel: BaseScreen
    {
        private readonly INavigationService _navigationService;

        public NewDatabaseViewModel( INavigationService navigationService)
            : base(navigationService)
        {
                _navigationService = navigationService;
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
            
            _navigationService.NavigateToViewModel<EntryGroupListViewModel>();
        }

        public XDocument NewDatabase()
        {
            var databaseXML = @"<KeePassFile>
                                  <Meta>
                                    <Generator>KeePass</Generator>
                                    <HeaderHash>0GuRCYFy2gzb1mm508FVJ1I0wDkJFHN3g0oN0GvSZ04=</HeaderHash>
                                    <DatabaseName>NewName</DatabaseName>
                                    <DatabaseNameChanged>2012-10-25T02:48:36Z</DatabaseNameChanged>
                                    <DatabaseDescription />
                                    <DatabaseDescriptionChanged>2012-10-25T02:47:56Z</DatabaseDescriptionChanged>
                                    <DefaultUserName />
                                    <DefaultUserNameChanged>2012-10-25T02:47:56Z</DefaultUserNameChanged>
                                    <MaintenanceHistoryDays>365</MaintenanceHistoryDays>
                                    <Color />
                                    <MasterKeyChanged>2012-10-25T02:47:56Z</MasterKeyChanged>
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
                                    <RecycleBinChanged>2012-10-25T02:47:56Z</RecycleBinChanged>
                                    <EntryTemplatesGroup>AAAAAAAAAAAAAAAAAAAAAA==</EntryTemplatesGroup>
                                    <EntryTemplatesGroupChanged>2012-10-25T02:47:56Z</EntryTemplatesGroupChanged>
                                    <HistoryMaxItems>10</HistoryMaxItems>
                                    <HistoryMaxSize>6291456</HistoryMaxSize>
                                    <LastSelectedGroup>GD11ELf89ESN7K5jV/HhAw==</LastSelectedGroup>
                                    <LastTopVisibleGroup>GD11ELf89ESN7K5jV/HhAw==</LastTopVisibleGroup>
                                    <Binaries />
                                    <CustomData />
                                  </Meta>
                                  <Root>
  
                                    <DeletedObjects />
                                  </Root>
                                </KeePassFile>";

            return XDocument.Parse(databaseXML);
        }
    }
}
