using System.Collections.Generic;
using System.Net;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using MetroPass.UI.Services;

namespace MetroPass.UI.ViewModels
{
    public class EntryGroupListViewModel : PasswordEntryScreen
    {
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<PwGroup> _entryGroupsWithEntries;
        private readonly IKdbTree _dbTree;

        public EntryGroupListViewModel(IKdbTree dbTree, INavigationService navigationService, IClipboard clipboard) : base(navigationService, clipboard)
        {
            _dbTree = dbTree;
            _navigationService = navigationService;
            _entryGroupsWithEntries = new ObservableCollection<PwGroup>();
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
                _entryGroupsWithEntries.Add(new PwGroup(value.Element, false));
                _entryGroupsWithEntries.AddRange(value.SubGroups);
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
            _navigationService.UriFor<GroupEditViewModel>().WithParam(vm => vm.GroupId, Root.UUID).Navigate();
        }


        public ObservableCollection<PwGroup> EntryGroupsWithEntries
        {
            get { return _entryGroupsWithEntries; }
        }

        public ObservableCollection<object> AllTogetherNow
        {
            get
            {
                var temp = new ObservableCollection<object>();

                temp.AddRange(Root.SubGroups);
                temp.AddRange(Root.Entries.OrderBy(e => e.Title));

                return temp;
            }
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
            _navigationService.UriFor<AddEntryViewModel>().WithParam(vm => vm.ParentGroupID, Root.UUID).Navigate();
        }

        public void AddGroup()
        {
            _navigationService.UriFor<AddGroupViewModel>().WithParam(vm => vm.ParentGroupID, Root.UUID).Navigate();
        }
    }
}