using System.Collections.Generic;
using System.Net;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using Windows.UI.Popups;

namespace MetroPass.UI.ViewModels
{
    public class AdGroup : IGroup
    {
        public string Name { get; set; }
        public AdGroup()
        {
            Name = "Advertisement";

        }
        public ObservableCollection<PwCommon> SubGroupsAndEntries
        {
            get
            {
                // TODO: Implement this property getter
                return new ObservableCollection<PwCommon>(){new AdItem()};
            }
        }
    }
    public class AdItem : PwCommon
    {

    }

    public class EntryGroupListViewModel : PasswordEntryScreen
    {
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<IGroup> _topLevelGroups;
        private readonly IKdbTree _dbTree;

        public EntryGroupListViewModel(IKdbTree dbTree, INavigationService navigationService, IClipboard clipboard) : base(navigationService, clipboard)
        {
            _dbTree = dbTree;
            _navigationService = navigationService;
            _topLevelGroups = new ObservableCollection<IGroup>();
        }

        private string _groupId;
        public string GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                var groupElement = _dbTree.FindGroupByUuid(value);
                Root = new PwGroup(groupElement);
            }
        }

        PwGroup _root = null;
        public PwGroup Root
        {
            get { return _root; }
            set
            {
                _root = value;
                _topLevelGroups.Add(new PwGroup(value.Element, false));
                _topLevelGroups.AddRange(value.SubGroups);
                _topLevelGroups.Add(new AdGroup());
                NotifyOfPropertyChange(() => Root);
            }
        }

        public void SelectGroup(PwGroup selectedGroup)
        {
            var encodedUUID = WebUtility.UrlEncode(selectedGroup.UUID);
            _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
        }

        public  void EditGroup()
        {
            var encodedUUID = WebUtility.UrlEncode(Root.UUID);
            _navigationService.UriFor<GroupEditViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
        }


        public ObservableCollection<IGroup> TopLevelGroups
        {
            get { return _topLevelGroups; }
        }

        public IEnumerable<PwGroup> GroupsOnThisLevel
        {
            get { return Root.SubGroups; }
        }

        public IEnumerable<PwEntry> EntriesOnThisLevel
        {
            get { return Root.Entries; }
        }

        public IEnumerable<PwCommon> FlatList
        {
            get { return GroupsOnThisLevel.Cast<PwCommon>().Union(EntriesOnThisLevel); }
        }

        public void AddEntry()
        {
            var encodedUUID = WebUtility.UrlEncode(Root.UUID);
            _navigationService.UriFor<AddEntryViewModel>().WithParam(vm => vm.ParentGroupID, encodedUUID).Navigate();
        }

        public void AddGroup()
        {
            var encodedUUID = WebUtility.UrlEncode(Root.UUID);
            _navigationService.UriFor<AddGroupViewModel>().WithParam(vm => vm.ParentGroupID, encodedUUID).Navigate();
        }

        public async void DeleteEntry()
        {
            var confirmMessage = String.Format("Are you sure you want to delete the password for {0}?", ((PwEntry)SelectedPasswordItem).Title);
            var confirmDialog = new MessageDialog(confirmMessage, "Confirm Delete");
            bool result = false;

            confirmDialog.Commands.Add(new UICommand("Yes", (cmd) => result = true));
            confirmDialog.Commands.Add(new UICommand("No", (cmd) => result = false));
            confirmDialog.DefaultCommandIndex = 0;
            confirmDialog.CancelCommandIndex = 1;

            await confirmDialog.ShowAsync();

            if (result)
            {
                SelectedPasswordItem.Element.Remove();
                ((PwEntry)SelectedPasswordItem).Remove();
                await PWDatabaseDataSource.Instance.SavePwDatabase();
            }
        }
    }
}