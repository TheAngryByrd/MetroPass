using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.Services.UI;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using ReactiveCaliburn;
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

        private readonly IDialogService _dialogService;

        private readonly IPWDatabaseDataSource _databaseSource;

        public OpenDatabaseViewModel(
            INavigationService navigationService,
            IDatabaseInfoRepository databaseInfoRepository,
            ICanSHA256Hash hasher,
            IDialogService dialogService,
            IPWDatabaseDataSource databaseSource)
        {
            _databaseSource = databaseSource;
            _dialogService = dialogService;
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

        private bool _progressIsVisible = false;
        public bool ProgressIsVisible
        {
            get { return _progressIsVisible; }
            set { this.RaiseAndSetIfChanged(ref _progressIsVisible, value); }
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

        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set { this.RaiseAndSetIfChanged(ref _progress, value); }
        }

        public ReactiveCommand OpenCommand { get; set; }
        private async void OpenDatabase(object obj)
        {
            ProgressIsVisible = true;
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

            try
            {
                await _databaseSource.LoadPwDatabase(file, listOfKeys, new Progress<double>(v => Progress = v));

                var rootUUID = _databaseSource.PwDatabase.Tree.Group.UUID;                

                _navigationService.UriFor<EntriesListViewModel>().
                    WithParam(p => p.GroupId, rootUUID).Navigate();
            }
            catch(Exception e)
            {
                _dialogService.ShowDialogBox("Error", "The password may be incorrect or the database may be corrupt.");
            }
            finally
            {
                ProgressIsVisible = false;
            }
         
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

        private async void ClearKeyFile(object obj)
        {           
            KeyFileName = string.Empty;
            await _databaseInfoRepository.DeleteKeyFile(_databaseInfo);
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
