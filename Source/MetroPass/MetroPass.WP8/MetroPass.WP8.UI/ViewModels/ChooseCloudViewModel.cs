using Caliburn.Micro;
using System;
using System.Collections.Generic;
using MetroPass.UI.DataModel;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using ReactiveCaliburn;
using ReactiveUI;
using Metropass.Core.PCL;
using Metropass.Core.PCL.Hashing;
using MetroPass.WP8.UI.DataModel;
using System.IO;
using MetroPass.WP8.UI.Services.UI;
using MetroPass.WP8.UI.Utils;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI.ViewModels
{
    public class ChooseCloudViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;
        private readonly ICanSHA256Hash _hasher;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;
        private readonly IDialogService _dialogService;        
        private readonly IPWDatabaseDataSource _databaseSource;

        private readonly ICache _cache;

        public ReactiveCommand NavigateToSkyDriveCommand { get; set; }

        public ReactiveCommand NavigateToDropboxCommand { get; set; }

        public ReactiveCommand NavigateToCreateDemoCommand { get; set; }

        public ChooseCloudViewModel(
            INavigationService navigationService,
            IDatabaseInfoRepository databaseInfoRepository,
            ICanSHA256Hash hasher,
            IDialogService dialogService,
            IPWDatabaseDataSource databaseSource,
            ICache cache)
        {
            _cache = cache;
            _databaseSource = databaseSource;
            _dialogService = dialogService;
            _databaseInfoRepository = databaseInfoRepository;
            _hasher = hasher;
            _navigationService = navigationService;

            NavigateToSkyDriveCommand = new ReactiveCommand();
            NavigateToSkyDriveCommand.Subscribe(NavigateToSkyDrive);
            
            NavigateToDropboxCommand = new ReactiveCommand();
            NavigateToDropboxCommand.Subscribe(NavigateToDropbox);

            NavigateToCreateDemoCommand = new ReactiveCommand();
            NavigateToCreateDemoCommand.Subscribe(NavigateToCreateDemo);
        }

        protected override async Task OnActivate()
        {
            DemoButtonIsVisible = _cache.DownloadFileNavigationCache.DownloadType == DownloadType.Database;
        }

        public string AddType 
        { 
            get
            {
                return _cache.DownloadFileNavigationCache.DownloadType.ToString().ToLower();
            }
        }

        private bool _demoButtonIsVisible;
        public bool DemoButtonIsVisible
        {
            get { return _demoButtonIsVisible; }
            set { this.RaiseAndSetIfChanged(ref _demoButtonIsVisible, value); }
        }

        private async void NavigateToCreateDemo(object obj)
        {
            var databaseXDocument = GenerateDatabase.NewDatabase();
            var parser = new Kdb4Parser(null);
            var passwordKey = await KcpPassword.Create("demo", _hasher);

            var compositeKey = new CompositeKey(new List<IUserKey> { passwordKey }, new NullableProgress<double>());
            var pwDatabase = new PwDatabase(compositeKey);



            pwDatabase.Tree = parser.ParseXmlDocument(databaseXDocument);
            pwDatabase.DataCipherUuid = new PwUuid(new byte[]{
						0x31, 0xC1, 0xF2, 0xE6, 0xBF, 0x71, 0x43, 0x50,
						0xBE, 0x58, 0x05, 0x21, 0x6A, 0xFC, 0x5A, 0xFF });


            _databaseSource.PwDatabase = pwDatabase;

            await _databaseInfoRepository.SaveDatabaseFromDatasouce("demo.kdbx", "", "", new MemoryStream());
            var databaseInfo = await _databaseInfoRepository.GetDatabaseInfo("demo.kdbx");

            _databaseSource.StorageFile = await databaseInfo.GetDatabase();
            await _databaseSource.SavePwDatabase();

            _dialogService.ShowDialogBox("Demo information", "The password to the database is 'demo'");

            _navigationService.GoBack();
        }

        private void NavigateToDropbox(object obj)
        {
            _navigationService.UriFor<DropboxAccessViewModel>().Navigate();
        }

        private void NavigateToSkyDrive(object obj)
        {
            _navigationService.UriFor<SkydriveAccessViewModel>().Navigate();
        }
    }
}
