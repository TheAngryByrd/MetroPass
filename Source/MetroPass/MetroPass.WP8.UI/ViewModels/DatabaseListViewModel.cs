﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.Interfaces;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using ReactiveUI;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DatabaseListViewModel : ReactiveScreen, IDatabaseListViewModel
    {
        private readonly INavigationService _navService;

        private readonly ICanSHA256Hash _hasher;

        private readonly IDatabaseInfoRepository _databaseInfoRepository;

        public DatabaseListViewModel(INavigationService navService, ICanSHA256Hash hasher, IDatabaseInfoRepository databaseInfoRepository)
        { 
            _databaseInfoRepository = databaseInfoRepository;
            _hasher = hasher;
            _navService = navService;
            DatabaseItems = new ObservableCollection<DatabaseItemViewModel>();
            NavigateToLoginCommand = new ReactiveCommand();
            NavigateToLoginCommand.Subscribe(a => ProgressIsVisible = true);
            NavigateToLoginCommand.Subscribe(NavigateToLogin);
            ProgressIsVisible = false;
        }

        public ObservableCollection<DatabaseItemViewModel> DatabaseItems
        {
            get;
            set;
        }

        public void AddDatabase()
        {
            _navService.UriFor<SkydriveAccessViewModel>().Navigate();
        }

        private bool _progressIsVisible;
        public bool ProgressIsVisible {
            get {
                return _progressIsVisible;
            }
            set {
                this.RaiseAndSetIfChanged(ref _progressIsVisible, value);
            }
        }

        protected async override void OnActivate()
        {
            ProgressIsVisible = false;
            var info = await _databaseInfoRepository.GetDatabaseInfo();

            DatabaseItems.AddRange(info.Select(i => new DatabaseItemViewModel(i)));
        }

        protected override void OnDeactivate(bool close)
        {
            DatabaseItems = new ObservableCollection<DatabaseItemViewModel>();
        }

        public ReactiveCommand NavigateToLoginCommand { get; private set; }

        public async void NavigateToLogin(object obj)
        {

            var installedFolder = Package.Current.InstalledLocation;


            var folder = await installedFolder.GetFolderAsync("SampleData");
            var file = await folder.GetFileAsync("Large.kdbx");
            var listOfKeys = new List<IUserKey>();
            listOfKeys.Add(await KcpPassword.Create("metropass",_hasher));

            await PWDatabaseDataSource.Instance.LoadPwDatabase(file, listOfKeys);

            var rootUUID = PWDatabaseDataSource.Instance.PwDatabase.Tree.Group.UUID;

            _navService.UriFor<EntriesListViewModel>().
                WithParam(p => p.GroupId, rootUUID).Navigate();
        }
    }



    public class DatabaseItemViewModel
    {
        public readonly DatabaseInfo DatabaseInfo;

        public DatabaseItemViewModel(DatabaseInfo databaseInfo)
        {
            DatabaseInfo = databaseInfo;
        }

        public string Name
        {
            get { return DatabaseInfo.Info.DatabasePath;}
        }
    }
}
