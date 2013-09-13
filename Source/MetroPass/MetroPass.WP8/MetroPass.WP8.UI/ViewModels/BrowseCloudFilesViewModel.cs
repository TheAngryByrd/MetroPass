using MetroPass.WP8.UI.Services.Cloud;
using MetroPass.WP8.UI.Services.Cloud.Skydrive;
using MetroPass.WP8.UI.Services.UI;
using MetroPass.WP8.UI.Utils;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.WP8.UI.DataModel;
using ReactiveCaliburn;
using ReactiveUI;

namespace MetroPass.WP8.UI.ViewModels
{
    public class BrowseCloudFilesViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;       

        private readonly ICloudProviderFactory _cloudFactory;
        private ICloudProviderAdapter _cloudProvider;

        public BrowseCloudFilesViewModel()
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

        public BrowseCloudFilesViewModel(
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

            this.ObservableForPropertyNotNull(vm => vm.SelectedSkyDriveItem)
                .Subscribe(SkydriveItemSelected);

        }

        private string _navigationUrl;
        public string NavigationUrl
        {
            get { return _navigationUrl; }
            set { _navigationUrl = value; }
        }

        public string DatabaseName { get; set; }

        public ObservableCollection<ICloudItem> SkyDriveItems { get; set; }

        private ICloudItem _selectedSkyDriveItem;
        public ICloudItem SelectedSkyDriveItem
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

       

        private void SkydriveItemSelected(IObservedChange<BrowseCloudFilesViewModel, ICloudItem> obj)
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
                    if (Cache.Instance.DownloadFileNavigationCache.DownloadType == DownloadType.Database)
                        await _databaseInfoRepository
                            .SaveDatabaseFromDatasouce(
                            cloudItem.Name,
                            CloudProvider.ToString(),
                            cloudItem.ID,
                            downloadStream);
                    else if (Cache.Instance.DownloadFileNavigationCache.DownloadType == DownloadType.KeyFile)
                        await _databaseInfoRepository
                            .SaveKeyFileFromDatasouce(
                            Cache.Instance.DownloadFileNavigationCache.DatabaseName, 
                            cloudItem.Name,
                            downloadStream);
                }
            }
            ProgressIsVisible = false;           
            Type returnNavigation = Type.GetType(Cache.Instance.DownloadFileNavigationCache.ReturnUrl);
            _navigationService.Navigate(GetUri(returnNavigation));
        }
  
        private void NavigateToBrowseFolders(ICloudItem cloudItem)
        {
            _navigationService.UriFor<BrowseCloudFilesViewModel>()
                              .WithParam(vm => vm.CloudProvider, CloudProvider)
                              .WithParam(vm => vm.NavigationUrl, cloudItem.ID)
                              .WithParam(vm => vm.DatabaseName, DatabaseName)
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

        public Uri GetUri(Type t)
        {
            Type viewType = Caliburn.Micro.ViewLocator.LocateTypeForModelType.Invoke(t, null, null);
            if (viewType == null)
            {
                throw new InvalidOperationException(string.Format("No view was found for {0}. See the log for searched views.", t.FullName));
            }
            string packUri = Caliburn.Micro.ViewLocator.DeterminePackUriFromType.Invoke(t, viewType);
 
            return new Uri(packUri, UriKind.Relative);
        }
    }
}
