using System.Linq;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using ReactiveUI;
using Caliburn.Micro;
using MetroPass.WP8.UI.DataModel;
using System.Windows;
using MetroPass.WP8.UI.Services;

namespace MetroPass.WP8.UI.ViewModels
{
    public class SkydriveBrowseFilesViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;       

        public SkydriveBrowseFilesViewModel()
        {
            if (Execute.InDesignMode)
            {
                SkyDriveItems = new ObservableCollection<SkyDriveItem>();
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
            IDatabaseInfoRepository databaseInfoRepository)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _databaseInfoRepository = databaseInfoRepository;
            ProgressIsVisible = true;
            
            SkyDriveItems = new ObservableCollection<SkyDriveItem>();

            this.ObservableForPropertyNotNull(vm => vm.SelectedSkyDriveItem).Subscribe(SkydriveItemSelected);            
        }

        private string _navigationUrl = "/me/skydrive/files";
        public string NavigationUrl
        {
            get { return _navigationUrl; }
            set { _navigationUrl = value; }
        }

        private LiveConnectClient _liveClient;

        public ObservableCollection<SkyDriveItem> SkyDriveItems { get; set; }

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

  
        private async Task AttemptDownload(SkyDriveItem skyDriveItem)
        {
            ProgressIsVisible = true;
            var operationResult = await _liveClient.DownloadAsync(skyDriveItem.ID + "/content");
            using (var downloadStream = operationResult.Stream)
            { 
                if (downloadStream != null)
                {
                    await _databaseInfoRepository.SaveDatabaseFromDatasouce(skyDriveItem.Name, downloadStream);
                }
            }
            ProgressIsVisible = false;

            _navigationService.UriFor<DatabaseListViewModel>()                             
                              .Navigate();
        }
  
        private void NavigateToBrowseFolders(SkyDriveItem value)
        {
            _navigationService.UriFor<SkydriveBrowseFilesViewModel>()
                              .WithParam(vm => vm.NavigationUrl, value.ID + "/files")
                              .Navigate();
        }

        protected async override void OnActivate()
        {
            SelectedSkyDriveItem = null;
            _liveClient = new LiveConnectClient(Cache.Instance.SkydriveSession);
            
            LiveOperationResult operationResult = await _liveClient.GetAsync(NavigationUrl);
            TryLoadItems(operationResult);
            ProgressIsVisible = false;
        }

        protected override void OnDeactivate(bool close)
        {
            SkyDriveItems = new ObservableCollection<SkyDriveItem>();                
        }

        private void TryLoadItems(LiveOperationResult operationResult)
        {
            dynamic result = operationResult.Result;
            if (result.data == null)
            {
                //this.ShowError("Server did not return a valid response.");
                return;
            }

            dynamic items = result.data;
            LoadItems(items);
        }

        private void LoadItems(dynamic items)
        {           
            var resultList = new List<SkyDriveItem>();            
            foreach (dynamic item in items)
            {
               resultList.Add(new SkyDriveItem(item));    
            }

            SkyDriveItems.AddRange(resultList);
        }    
    }

    public class SkyDriveItem
    {
        public SkyDriveItem(IDictionary<string, object> properties)
        {
            if (properties.ContainsKey("id"))
            {
                this.ID = properties["id"] as string;
            }

            if (properties.ContainsKey("name"))
            {
                this.Name = properties["name"] as string;
            }

            if (properties.ContainsKey("type"))
            {
                this.ItemType = properties["type"] as string;
            }
        }

        public SkyDriveItem(string Id, string name, string itemType)
        {
            ID = Id;
            Name = name;
            ItemType = itemType;
        }

        public string ID { get; private set; }

        public string Name { get; private set; }

        public string ItemType { get; private set; }

        public bool IsFolder
        {
            get
            {
                return !string.IsNullOrEmpty(this.ItemType) &&
                       (this.ItemType.Equals("folder") || this.ItemType.Equals("album"));
            }
        }

        private string[] knownFileTypes = new[] { "kdbx", "key" };

        public bool IsKeePassItem
        {
            get
            {
                var filetype = Name.Split('.').Last();
                return knownFileTypes.Contains(filetype);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

}
