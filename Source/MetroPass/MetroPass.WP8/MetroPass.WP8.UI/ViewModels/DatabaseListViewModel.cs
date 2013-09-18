using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.Services.UI;
using MetroPass.WP8.UI.ViewModels.Interfaces;
using ReactiveCaliburn;
using ReactiveUI;
using MetroPass.WP8.UI.Utils;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DatabaseListViewModel : ReactiveScreen, IDatabaseListViewModel
    {
        private readonly INavigationService _navService;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;

        private readonly ICache _cache;

        private readonly IDialogService _dialogService;

        public DatabaseListViewModel(INavigationService navService,
            IDatabaseInfoRepository databaseInfoRepository,
            ICache cache,
            IDialogService dialogService)
        {
            _dialogService = dialogService;
            _cache = cache;
            _databaseInfoRepository = databaseInfoRepository;

            _navService = navService;
            DatabaseItems = new ObservableCollection<DatabaseItemViewModel>();   

            DeleteDatabaseCommand = new ReactiveCommand();
            DeleteDatabaseCommand.Subscribe(DeleteDatabase);

            this.ObservableForPropertyNotNull(vm => vm.SelectedDatabaseItem).Subscribe(NavigateToOpenDatabase);
        }

        private void NavigateToOpenDatabase(IObservedChange<DatabaseListViewModel, DatabaseItemViewModel> obj)
        {
            _cache.DatabaseName = obj.Value.DatabaseInfo.Info.DatabasePath;
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
            _cache.DownloadFileNavigationCache = new DownloadFileNavigationCache
            {                
                DownloadType = DownloadType.Database,
                ReturnUrl = this.GetType().FullName
            };
            _navService.UriFor<ChooseCloudViewModel>().Navigate();
        }

        protected async override void OnActivate()
        {
            DatabaseItems.Clear();
            SelectedDatabaseItem = null;

            var info = await _databaseInfoRepository.GetDatabaseInfo();

            DatabaseItems.AddRange(info.Select(i => new DatabaseItemViewModel(i)));  
         
            if(_cache.ShowIntroDropboxMessage)
            {
                _cache.ShowIntroDropboxMessage = false;
                _dialogService.ShowDialogBox("Dropbox users", "Dropbox has changed usage permissions.  As a result you will have to download your database again. Sorry for the inconvenience.");
            }
        }

        public ReactiveCommand DeleteDatabaseCommand { get; set; }
        public async void DeleteDatabase(object obj)
        {
            var databaseItem = obj as DatabaseItemViewModel;            
            DatabaseItemViewModel single = DatabaseItems.SingleOrDefault(d => databaseItem.Name == d.Name);

            if (single != null)
            {
                DatabaseItems.Remove(single);
                await databaseItem.DatabaseInfo.Folder.DeleteAsync();
            }
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
