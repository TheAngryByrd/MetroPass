using System.Collections.Generic;
using Caliburn.Micro;
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

        public EntryGroupListViewModel(INavigationService navigationService, IClipboard clipboard) : base(navigationService, clipboard)
        {
            _navigationService = navigationService;
            _entryGroupsWithEntries = new ObservableCollection<PwGroup>();
        }

        PwGroup _root = null;
        public PwGroup Root
        {
            get { return _root; }
            set
            {
                _root = value;
                _entryGroupsWithEntries.Add(new PwGroup(value.Element, value.Entries.OrderBy(e => e.Title)));
                _entryGroupsWithEntries.AddRange(value.SubGroups);
                NotifyOfPropertyChange(() => Root);
            }
        }

        public void SelectGroup(PwGroup selectedGroup)
        {
            _navigationService.NavigateToViewModel<EntryGroupListViewModel, PwGroup>(selectedGroup, vm => vm.Root);
        }

        public  void EditGroup()
        {
            _navigationService.NavigateToViewModel<GroupEditViewModel, PwGroup>(Root, vm => vm.Group);
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
    }
}