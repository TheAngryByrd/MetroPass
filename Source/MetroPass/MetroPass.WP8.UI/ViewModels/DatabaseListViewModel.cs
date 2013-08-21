using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Caliburn.Micro;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels.DesignTime;
using MetroPass.WP8.UI.ViewModels.Interfaces;
using ReactiveUI;
using Windows.ApplicationModel;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DatabaseListViewModel : IDatabaseListViewModel
    {
        private readonly INavigationService _navService;

        private readonly ICanSHA256Hash _hasher;

        public DatabaseListViewModel(INavigationService navService, ICanSHA256Hash hasher)
        { 
            _hasher = hasher;
            _navService = navService;
            DatabaseNames = new ObservableCollection<string> { "Personal", "Work" };
            NavigateToLoginCommand = new ReactiveCommand();
            NavigateToLoginCommand.Subscribe(NavigateToLogin);
        }

        public ObservableCollection<string> DatabaseNames
        {
            get;
            set;
        }

        public IReactiveCommand NavigateToLoginCommand { get; private set; }

        public async void NavigateToLogin(object obj)
        {
            var installedFOlder = Package.Current.InstalledLocation;

            var folder = await installedFOlder.GetFolderAsync("SampleData");
            var file = await folder.GetFileAsync("Large.kdbx");
            var listOfKeys = new List<IUserKey>();
            listOfKeys.Add(await KcpPassword.Create("metropass",_hasher));

            await PWDatabaseDataSource.Instance.LoadPwDatabase(file, listOfKeys, new NullableProgress<double>());

            _navService.UriFor<EntriesListViewModel>().Navigate();
        }
    }
}
