using System;
using System.Collections.Generic;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.Views;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using Windows.Storage.Pickers;

namespace MetroPass.UI.ViewModels
{
    public class NewDatabaseViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;

        private readonly IPageServices _pageServices;

        private readonly IPWDatabaseDataSource _dataSource;

        public NewDatabaseViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPageServices pageServices,
            IPWDatabaseDataSource dataSource) :
            base(navigationService, eventAggregator, pageServices)
        {
            _dataSource = dataSource;
            this._pageServices = pageServices;
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
            var databaseXDocument = GenerateDatabase.NewDatabase();
            var parser = new Kdb4Parser(null);
            var passwordKey = await KcpPassword.Create(Password, new SHA256HasherRT());

            var compositeKey = new CompositeKey(new List<IUserKey> { passwordKey }, new NullableProgress<double>());
            var pwDatabase = new PwDatabase(compositeKey);



            pwDatabase.Tree = parser.ParseXmlDocument(databaseXDocument);
            pwDatabase.DataCipherUuid = new PwUuid(new byte[]{
						0x31, 0xC1, 0xF2, 0xE6, 0xBF, 0x71, 0x43, 0x50,
						0xBE, 0x58, 0x05, 0x21, 0x6A, 0xFC, 0x5A, 0xFF });


            if (await _pageServices.EnsureUnsnapped())
            {
                FileSavePicker saver = new FileSavePicker();
                saver.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                saver.SuggestedFileName = this.Name;
                saver.FileTypeChoices.Add(".kdbx", new List<string> { ".kdbx" });
                var storageFile = await saver.PickSaveFileAsync();

                if (storageFile != null)
                {

                    _dataSource.PwDatabase = pwDatabase;
                    _dataSource.StorageFile = storageFile;
                    await _dataSource.SavePwDatabase();
                    var encodedUUID = _dataSource.PwDatabase.Tree.Group.UUID;
                    _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
                }
            }


        }

    }
}
