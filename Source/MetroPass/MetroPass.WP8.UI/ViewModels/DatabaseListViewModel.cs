using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.Interfaces;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using ReactiveUI;
using Windows.ApplicationModel;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DatabaseListViewModel : ReactiveScreen,IDatabaseListViewModel
    {
        private readonly INavigationService _navService;

        private readonly ICanSHA256Hash _hasher;

        public DatabaseListViewModel(INavigationService navService, ICanSHA256Hash hasher)
        { 
            _hasher = hasher;
            _navService = navService;
            DatabaseNames = new ObservableCollection<string> { "Personal", "Work" };
            NavigateToLoginCommand = new ReactiveCommand();
            NavigateToLoginCommand.Subscribe(a => ProgressIsVisible = true);
            NavigateToLoginCommand.Subscribe(NavigateToLogin);
            ProgressIsVisible = false;
        }

        public ObservableCollection<string> DatabaseNames
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

        protected override void OnActivate()
        {
            ProgressIsVisible = false;
        }

        public ReactiveCommand NavigateToLoginCommand { get; private set; }

        public async void NavigateToLogin(object obj)
        {           
            var installedFOlder = Package.Current.InstalledLocation;

            var folder = await installedFOlder.GetFolderAsync("SampleData");
            var file = await folder.GetFileAsync("Large.kdbx");
            var listOfKeys = new List<IUserKey>();
            listOfKeys.Add(await KcpPassword.Create("metropass",_hasher));

            await PWDatabaseDataSource.Instance.LoadPwDatabase(file, listOfKeys);

            var rootUUID = PWDatabaseDataSource.Instance.PwDatabase.Tree.Group.UUID;

            _navService.UriFor<EntriesListViewModel>().
                WithParam(p => p.GroupId, rootUUID).Navigate();
        }
    }

}
