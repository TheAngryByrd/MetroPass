using System.Collections.ObjectModel;
using MetroPass.UI.DataModel;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.Services.Cloud;
using Metropass.Core.PCL.Model;
using ReactiveCaliburn;
using ReactiveUI;
using Caliburn.Micro;
using System;
using MetroPass.WP8.UI.Utils;
using System.IO;
using MetroPass.WP8.UI.Services.UI;

namespace MetroPass.WP8.UI.ViewModels
{
   

    public class EntriesListViewModel : ReactiveScreen
    {

        private readonly INavigationService _navigationService;
        private readonly IPWDatabaseDataSource _databaseSource;
        private readonly ICloudProviderFactory _cloudProvider;
        private readonly ICache _cache;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;
        private readonly IDialogService _dialogService;        

        public EntriesListViewModel(INavigationService navigationService,
            IPWDatabaseDataSource databaseSource,
            ICloudProviderFactory cloudProvider,
            ICache cache,
            IDatabaseInfoRepository databaseInfoRepository,
            IDialogService dialogService)
        {
            _dialogService = dialogService;
            _databaseInfoRepository = databaseInfoRepository;
            _cache = cache;
            _cloudProvider = cloudProvider;
            _databaseSource = databaseSource;
            _navigationService = navigationService;   
            this.ObservableForProperty(vm => vm.GroupId)
                .Subscribe(x => GetGroup(x.Value));
            this.ObservableForProperty(vm => vm.SelectedItem)                
                .Subscribe(NavigateToEntriesListView);
            Items = new ObservableCollection<PwCommon>();
        }

        private bool _progressIsVisible = false;
        public bool ProgressIsVisible
        {
            get { return _progressIsVisible; }
            set { this.RaiseAndSetIfChanged(ref _progressIsVisible, value); }
        }

        private void NavigateToEntriesListView(IObservedChange<EntriesListViewModel, PwCommon> obj)
        {
            if (obj.Value is PwGroup)
            {
                _navigationService.UriFor<EntriesListViewModel>().WithParam(vm => vm.GroupId, obj.Value.UUID).Navigate();
            }
            else if(obj.Value is PwEntry)
            {
                var entry = obj.Value as PwEntry;
                _navigationService
                    .UriFor<AddOrEditEntryViewModel>()
                    .WithParam(vm => vm.EntryUuid, entry.UUID)
                    .WithParam(vm => vm.ParentGroupUuid, entry.ParentGroup.UUID)
                    .Navigate();
            }
        }

        public void AddEntry()
        {
            _navigationService
                   .UriFor<AddOrEditEntryViewModel>()
                   .WithParam(vm => vm.ParentGroupUuid, Group.UUID)
                   .Navigate();
        }

        public void AddGroup()
        {
            _navigationService
                   .UriFor<AddOrEditGroupViewModel>()
                   .WithParam(vm => vm.ParentGroupUuid, Group.UUID)
                   .Navigate();
        }

        public async void Upload()
        {
            ProgressIsVisible = true;
            var info = await _databaseInfoRepository.GetDatabaseInfo(_cache.DatabaseName);
            var cloudProviderEnum = info.Info.DatabaseCloudProvider;
            if(!string.IsNullOrWhiteSpace(cloudProviderEnum))
            {
                var cloudProvider = await _cloudProvider.GetCloudProvider(cloudProviderEnum);
                using (var fileToWrite = await _databaseSource.StorageFile.OpenStreamForReadAsync())
                {
                    await cloudProvider.Upload(info.Info.DatabaseUploadCloudPath, _cache.DatabaseName, fileToWrite);
                }
            }
            else
            {
                _dialogService.ShowDialogBox("I'm sorry Dave, I'm afraid I can't do that", "You can only upload a file that has been previously downloaded from skydrive or dropbox");
            }
            ProgressIsVisible = false;
        }

        public ObservableCollection<PwCommon> Items
        {
            get;
            set;
        }

        private PwCommon _selectedItem;
        public PwCommon SelectedItem {
            get { return _selectedItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        }

        protected override void OnActivate()
        {
            Items.Clear();
            SelectedItem = null;
            GetGroup(GroupId);
            Items.AddRange(Group.SubGroupsAndEntries);        
        }
        protected override void OnDeactivate(bool close)
        {            
           
        }

        private void GetGroup(string groupId)
        {
            Group = _databaseSource.PwDatabase.Tree.FindGroupByUuid(groupId);                     
        }    

        const string Key = "\uE192";
        const string Folder = "\uE1C1";

        private string _groupId;
        public string GroupId {
            get {
                return _groupId;
            }
            set {
                this.RaiseAndSetIfChanged(ref _groupId,value);
            }
        }

        private PwGroup _group;
        public PwGroup Group {
            get {
                return _group;
            }
            set {
                this.RaiseAndSetIfChanged(ref _group, value);
            }
        }

        public string PageTitle
        {
            get
            {
                return Group.Name;
            }
        }
    }
}
