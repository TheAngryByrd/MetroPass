using System.Collections.ObjectModel;
using MetroPass.UI.DataModel;
using Metropass.Core.PCL.Model;
using ReactiveCaliburn;
using ReactiveUI;
using Caliburn.Micro;
using System;

namespace MetroPass.WP8.UI.ViewModels
{
   

    public class EntriesListViewModel : ReactiveScreen
    {      

        private readonly INavigationService _navigationService;

        private readonly IPWDatabaseDataSource _databaseSource;

        public EntriesListViewModel(INavigationService navigationService,
            IPWDatabaseDataSource databaseSource)
        {
            _databaseSource = databaseSource;
            _navigationService = navigationService;   
            this.ObservableForProperty(vm => vm.GroupId)
                .Subscribe(x => GetGroup(x.Value));
            this.ObservableForProperty(vm => vm.SelectedItem)                
                .Subscribe(NavigateToEntriesListView);
            Items = new ObservableCollection<PwCommon>();
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
