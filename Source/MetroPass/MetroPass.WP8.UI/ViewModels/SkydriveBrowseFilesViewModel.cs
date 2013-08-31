using System.Linq;
using MetroPass.WP8.UI.Services.UI;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ReactiveUI;
using Caliburn.Micro;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.Services;

namespace MetroPass.WP8.UI.ViewModels
{

    public class SkydriveBrowseFilesViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;       

        private readonly ICloudProviderFactory _cloudFactory;
        private ICloudProviderAdapter _cloudProvider;

        public SkydriveBrowseFilesViewModel()
        {
            if (Execute.InDesignMode)
            {
                SkyDriveItems = new ObservableCollection<ICloudItem>();
                SkyDriveItems.Add(new SkyDriveItem("Id","Documents", "folder"));
                SkyDriveItems.Add(new SkyDriveItem("Id","Pictures", "folder"));
                SkyDriveItems.Add(new SkyDriveItem("Id","Downloads", "folder"));
                SkyDriveItems.Add(new SkyDriveItem("Id","Music", "folder"));
                SkyDriveItems.Add(new SkyDriveItem("Id","Word doc 1.doc", "file"));
                SkyDriveItems.Add(new SkyDriveItem("Id","Word doc 1.doc", "file"));
                SkyDriveItems.Add(new SkyDriveItem("Id","database.kdbx", "file"));
                SkyDriveItems.Add(new SkyDriveItem("Id","Word doc 1.doc", "file"));
            }
        }

        public SkydriveBrowseFilesViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IDatabaseInfoRepository databaseInfoRepository,
            ICloudProviderFactory factory)
        {
            _cloudFactory = factory;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _databaseInfoRepository = databaseInfoRepository;
            ProgressIsVisible = true;
            
            SkyDriveItems = new ObservableCollection<ICloudItem>();

            this.ObservableForPropertyNotNull(vm => vm.SelectedSkyDriveItem).Subscribe(SkydriveItemSelected);            
        }

        private string _navigationUrl = "/me/skydrive";
        public string NavigationUrl
        {
            get { return _navigationUrl; }
            set { _navigationUrl = value; }
        }

        public ObservableCollection<ICloudItem> SkyDriveItems { get; set; }

        private SkyDriveItem _selectedSkyDriveItem;
        public SkyDriveItem SelectedSkyDriveItem
        {
            get { return _selectedSkyDriveItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedSkyDriveItem, value); }
        }

        private bool _progressIsVisible;
        public bool ProgressIsVisible
        {
            get { return _progressIsVisible; }
            set { this.RaiseAndSetIfChanged(ref _progressIsVisible, value); }
        }

        public CloudProvider CloudProvider { get; set; }

       

        private void SkydriveItemSelected(IObservedChange<SkydriveBrowseFilesViewModel, SkyDriveItem> obj)
        {
            var value = obj.Value;

            if (value.IsFolder) 
            { 
                NavigateToBrowseFolders(value);
            }
            else
            { 
                _dialogService.ShowDialogBox(
                    "Download Confirmation",
                    string.Format("Are you sure you want to download {0}?", value.Name),
                    "yes",
                    "no",
                    async () => { await AttemptDownload(value); },
                    () => { SelectedSkyDriveItem = null; }
                );
            }
        }

  
        private async Task AttemptDownload(ICloudItem cloudItem)
        {
            ProgressIsVisible = true;
            
            using (var downloadStream = await _cloudProvider.DownloadItem(cloudItem.ID))
            { 
                if (downloadStream != null)
                {
                    await _databaseInfoRepository.SaveDatabaseFromDatasouce(cloudItem.Name, downloadStream);
                }
            }
            ProgressIsVisible = false;

            _navigationService.UriFor<DatabaseListViewModel>()                             
                              .Navigate();
        }
  
        private void NavigateToBrowseFolders(ICloudItem value)
        {
            _navigationService.UriFor<SkydriveBrowseFilesViewModel>()
                              .WithParam(vm => vm.NavigationUrl, value.ID + "/files")
                              .Navigate();
        }

        protected async override void OnActivate()
        {
            _cloudProvider = _cloudFactory.GetCloudProvider(CloudProvider);

            SelectedSkyDriveItem = null;
            
            var items = await _cloudProvider.GetItems(NavigationUrl);

            SkyDriveItems.AddRange(items);
            ProgressIsVisible = false;
        }

        protected override void OnDeactivate(bool close)
        {
            SkyDriveItems = new ObservableCollection<ICloudItem>();                
        }    
    }
}
