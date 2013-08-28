using System;
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
using MetroPass.WP8.UI.Utils;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DatabaseListViewModel : ReactiveScreen, IDatabaseListViewModel
    {
        private readonly INavigationService _navService;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;

        public DatabaseListViewModel(INavigationService navService, 
            IDatabaseInfoRepository databaseInfoRepository)
        { 
            _databaseInfoRepository = databaseInfoRepository;

            _navService = navService;
            DatabaseItems = new ObservableCollection<DatabaseItemViewModel>();      
            ProgressIsVisible = false;


            this.ObservableForPropertyNotNull(vm => vm.SelectedDatabaseItem).Subscribe(NavigateToOpenDatabase);
        }

        private void NavigateToOpenDatabase(IObservedChange<DatabaseListViewModel, DatabaseItemViewModel> obj)
        {
            Cache.Instance.DatabaseInfo = obj.Value.DatabaseInfo;
            _navService.UriFor<OpenDatabaseViewModel>().Navigate();            
        }

        private DatabaseItemViewModel _selectedDatabaseItem;
        public DatabaseItemViewModel SelectedDatabaseItem
        {
            get { return _selectedDatabaseItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedDatabaseItem, value); }
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
            SelectedDatabaseItem = null;

            var info = await _databaseInfoRepository.GetDatabaseInfo();

            DatabaseItems.AddRange(info.Select(i => new DatabaseItemViewModel(i)));
        }

        protected override void OnDeactivate(bool close)
        {
            DatabaseItems = new ObservableCollection<DatabaseItemViewModel>();
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
