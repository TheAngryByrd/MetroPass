using Caliburn.Micro;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using ReactiveUI;
using System;
using System.Collections.Generic;
using Metropass.Core.PCL.Hashing;
using MetroPass.WP8.UI.Utils;
using PCLStorage;

namespace MetroPass.WP8.UI.ViewModels
{
    public class OpenDatabaseViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;
        private readonly ICanSHA256Hash _hasher;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;

        public OpenDatabaseViewModel(
            INavigationService navigationService,
            IDatabaseInfoRepository databaseInfoRepository,
            ICanSHA256Hash hasher)
        {
            _databaseInfoRepository = databaseInfoRepository;
            _hasher = hasher;
            _navigationService = navigationService;

            var canHitOpen = this.WhenAny(
                vm => vm.Password, 
                vm => vm.KeyFileName,
                (p, k) => !string.IsNullOrEmpty(p.Value) || !string.IsNullOrEmpty(k.Value));

            OpenCommand = new ReactiveCommand(canHitOpen);
            OpenCommand.Subscribe(OpenDatabase);

            GetKeyFileCommand = new ReactiveCommand();
            GetKeyFileCommand.Subscribe(GetKeyFile); 
            
            ClearKeyFileCommand = new ReactiveCommand();
            ClearKeyFileCommand.Subscribe(ClearKeyFile);

            IObservable<string> keyFileNameChanged = this.WhenAny(vm => vm.KeyFileName, kf => kf.Value);
            keyFileNameChanged.Subscribe(v => ClearKeyFileButtonIsVisible = !string.IsNullOrWhiteSpace(v));
            keyFileNameChanged.Subscribe(v => GetKeyFileButtonIsVisible = string.IsNullOrWhiteSpace(v));
        }

        private DatabaseInfo _databaseInfo;

        protected async override void OnActivate()
        {
            _databaseInfo = await _databaseInfoRepository.GetDatabaseInfo(Cache.Instance.DatabaseName);

            DatabaseName = _databaseInfo.Info.DatabasePath;
            if (!string.IsNullOrWhiteSpace(_databaseInfo.Info.KeyFilePath))
            {
                KeyFileName = _databaseInfo.Info.KeyFilePath;
            }
        }

        public ReactiveCommand OpenCommand { get; set; }
        private async void OpenDatabase(object obj)
        {

            var file = await _databaseInfo.GetDatabase();
            var listOfKeys = new List<IUserKey>();

            if (!string.IsNullOrEmpty(Password))
            {
                KcpPassword password = await KcpPassword.Create(Password, _hasher);
                listOfKeys.Add(password);
            }
                
            if(!string.IsNullOrWhiteSpace(KeyFileName))
            {
                var keyFile = await _databaseInfo.GetKeyfile();
                var kcpKeyFile = await KcpKeyFile.Create(new WP8File(keyFile), _hasher);
                listOfKeys.Add(kcpKeyFile);
            }

            await PWDatabaseDataSource.Instance.LoadPwDatabase(file, listOfKeys);

            var rootUUID = PWDatabaseDataSource.Instance.PwDatabase.Tree.Group.UUID;

            _navigationService.UriFor<EntriesListViewModel>().
                WithParam(p => p.GroupId, rootUUID).Navigate();
        }

        private bool _getKeyFileButtonIsVisible;
        public bool GetKeyFileButtonIsVisible
        {
            get { return _getKeyFileButtonIsVisible; }
            set { this.RaiseAndSetIfChanged(ref _getKeyFileButtonIsVisible, value); }
        }

        public ReactiveCommand GetKeyFileCommand { get; private set; }
        private void GetKeyFile(object obj)
        {
            Cache.Instance.DownloadFileNavigationCache = new DownloadFileNavigationCache
            {
                DatabaseName = DatabaseName,
                DownloadType = DownloadType.KeyFile,
                ReturnUrl = this.GetType().FullName
            };
            _navigationService.UriFor<ChooseCloudViewModel>().Navigate();           
        }

        private bool _clearKeyFileButtonIsVisible;
        public bool ClearKeyFileButtonIsVisible
        {
            get { return _clearKeyFileButtonIsVisible; }
            set { this.RaiseAndSetIfChanged(ref _clearKeyFileButtonIsVisible, value); }
        }

        public ReactiveCommand ClearKeyFileCommand { get; private set; }

        private void ClearKeyFile(object obj)
        {
            KeyFileName = string.Empty;
        }




        private string _databaseName;
        public string DatabaseName
        {
            get { return _databaseName; }
            set { this.RaiseAndSetIfChanged(ref _databaseName, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }


        private string _keyFileName;
        public string KeyFileName
        {
            get { return _keyFileName; }
            set { this.RaiseAndSetIfChanged(ref _keyFileName, value); }
        }
    }
}
