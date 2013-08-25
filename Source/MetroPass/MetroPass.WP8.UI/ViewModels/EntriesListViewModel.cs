﻿using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Media;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Metropass.Core.PCL.Model;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using Caliburn.Micro;

namespace MetroPass.WP8.UI.ViewModels
{
   

    public class EntriesListViewModel : ReactiveScreen
    {      

        private readonly INavigationService _navigationService;

        public EntriesListViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.ObservableForProperty(vm => vm.GroupId).Subscribe(GetGroup);
            this.ObservableForProperty(vm => vm.SelectedItem)
                .Where(v => v.Value is PwGroup)
                .Subscribe(NavigateToEntriesListView);
            Items = new ObservableCollection<PwCommon>();
        }     

        private void NavigateToEntriesListView(IObservedChange<EntriesListViewModel, PwCommon> obj)
        {
            _navigationService.UriFor<EntriesListViewModel>().WithParam(vm => vm.GroupId, obj.Value.UUID).Navigate();
        }

        public ObservableCollection<PwCommon> Items
        {
            get;
            set;
        }

        private PwCommon _selectedItem;
        public PwCommon SelectedItem {
            get {
                return _selectedItem;
            }
            set {
                this.RaisePropertyChanging();
                _selectedItem = value;
                this.RaisePropertyChanged();
            }
        }

        protected override void OnActivate()
        {
            SelectedItem = null;
            Items.AddRange(Group.SubGroupsAndEntries);        
        }
        protected override void OnDeactivate(bool close)
        {            
            Items = new ObservableCollection<PwCommon>();
        }

        private void GetGroup(IObservedChange<EntriesListViewModel, string> obj)
        {            
            Group = PWDatabaseDataSource.Instance.PwDatabase.Tree.FindGroupByUuid(obj.Value);                     
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
    }
}
