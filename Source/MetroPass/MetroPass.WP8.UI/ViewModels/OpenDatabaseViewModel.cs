﻿using Caliburn.Micro;
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

        public OpenDatabaseViewModel(INavigationService navigationService,
            ICanSHA256Hash hasher)
        {
            _hasher = hasher;
            _navigationService = navigationService;

            var canHitOpen = this.WhenAny(
                vm => vm.Password, 
                vm => vm.KeyFileName,
                (p, k) => !string.IsNullOrEmpty(p.Value) || !string.IsNullOrEmpty(k.Value));

            OpenCommand = new ReactiveCommand(canHitOpen);
            OpenCommand.Subscribe(OpenDatabase);
        }

        protected override void OnActivate()
        {
            DatabaseName = Cache.Instance.DatabaseInfo.Info.DatabasePath;
            if(!string.IsNullOrWhiteSpace(Cache.Instance.DatabaseInfo.Info.KeyFilePath))
            {
                KeyFileName = Cache.Instance.DatabaseInfo.Info.KeyFilePath;
            }
        }

        public ReactiveCommand OpenCommand { get; set; }
        private async void OpenDatabase(object obj)
        {
            
            var file = await Cache.Instance.DatabaseInfo.GetDatabase();
            var listOfKeys = new List<IUserKey>();

            if (!string.IsNullOrEmpty(Password))
            {
                KcpPassword password = await KcpPassword.Create(Password, _hasher);
                listOfKeys.Add(password);
            }
                
            if(!string.IsNullOrWhiteSpace(KeyFileName))
            {
                var keyFile = await Cache.Instance.DatabaseInfo.GetKeyfile();
                var kcpKeyFile = await KcpKeyFile.Create(new WP8File(keyFile), _hasher);
                listOfKeys.Add(kcpKeyFile);
            }

            await PWDatabaseDataSource.Instance.LoadPwDatabase(file, listOfKeys);

            var rootUUID = PWDatabaseDataSource.Instance.PwDatabase.Tree.Group.UUID;

            _navigationService.UriFor<EntriesListViewModel>().
                WithParam(p => p.GroupId, rootUUID).Navigate();
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
