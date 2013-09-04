using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.Interfaces;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using ReactiveUI;
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

            DeleteDatabaseCommand = new ReactiveCommand();
            DeleteDatabaseCommand.Subscribe(DeleteDatabase);


            this.ObservableForPropertyNotNull(vm => vm.SelectedDatabaseItem).Subscribe(NavigateToOpenDatabase);
        }

        private void NavigateToOpenDatabase(IObservedChange<DatabaseListViewModel, DatabaseItemViewModel> obj)
        {
            Cache.Instance.DatabaseName = obj.Value.DatabaseInfo.Info.DatabasePath;
            _navService.UriFor<OpenDatabaseViewModel>()
                .Navigate();            
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
            Cache.Instance.DownloadFileNavigationCache = new DownloadFileNavigationCache
            {                
                DownloadType = DownloadType.Database,
                ReturnUrl = this.GetType().FullName
            };
            _navService.UriFor<ChooseCloudViewModel>().Navigate();
        }

        protected async override void OnActivate()
        {          
            SelectedDatabaseItem = null;

            var info = await _databaseInfoRepository.GetDatabaseInfo();

            DatabaseItems.AddRange(info.Select(i => new DatabaseItemViewModel(i)));
        }

        protected override void OnDeactivate(bool close)
        {
            DatabaseItems = new ObservableCollection<DatabaseItemViewModel>();
        }

        public ReactiveCommand DeleteDatabaseCommand { get; set; }
        public async void DeleteDatabase(object obj)
        {
            var databaseItem = obj as DatabaseItemViewModel;
            
            await databaseItem.DatabaseInfo.Folder.DeleteAsync();
            DatabaseItems.Remove(databaseItem);
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
