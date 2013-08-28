﻿using System.Reactive.Linq;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Caliburn.Micro;
using System.Linq.Expressions;
using Windows.ApplicationModel;
using Windows.Storage;
using MetroPass.WP8.UI.DataModel;

namespace MetroPass.WP8.UI.ViewModels
{

    public static class ObservableEx
    {
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForPropertyNotNull<TSender, TValue>(this TSender This, Expression<Func<TSender, TValue>> property, Boolean beforeChange = false, Boolean skipInitial = true)
        {
            if (This == null)
            {
                throw new ArgumentNullException("Sender");
            }
            String[] propertyNames = Reflection.ExpressionToPropertyNames<TSender, TValue>(property);
            return This.SubscribeToExpressionChain<TSender, TValue>(propertyNames, beforeChange, skipInitial).Where(o => o.Value != null);
        }
    }
    public class SkydriveBrowseFilesViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;
        private string _navigationUrl = "/me/skydrive/files";   
        private readonly IDatabaseInfoRepository _databaseInfoRepository;

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

        public SkydriveBrowseFilesViewModel(INavigationService navigationService, IDatabaseInfoRepository databaseInfoRepository)
        {
            _databaseInfoRepository = databaseInfoRepository;
            ProgressIsVisible = true;
            _navigationService = navigationService;

            SkyDriveItems = new ObservableCollection<SkyDriveItem>();

            this.ObservableForPropertyNotNull(vm => vm.SelectedSkyDriveItem).Subscribe(SkydriveItemSelected);
        }

        private async void SkydriveItemSelected(IObservedChange<SkydriveBrowseFilesViewModel, SkyDriveItem> obj)
        {
            var value = obj.Value;

            if (value.IsFolder) 
            { 
                NavigateToBrowseFolders(value);
            }
            else
            { 
                await AttemptDownload(value);
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

        public override string ToString()
        {
            return this.Name;
        }
    }

}
