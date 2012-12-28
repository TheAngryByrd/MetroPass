using Caliburn.Micro;
using Framework;
using MetroPass.Core.Exceptions;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Windows.UI.Popups;

namespace MetroPass.UI.ViewModels
{
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
                try {
                    var groupElement = _dbTree.FindGroupByUuid(value);
                    Root = new PwGroup(groupElement);
                }
                catch (GroupNotFoundException) {
                    Root = PwGroup.NullGroup;
                    QueueState("GroupNotFound");
                }
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

        private bool PasswordIsInRecycleBin
        {
            get 
            {
                return SelectedPasswordItem != null &&
                    ((PwEntry)SelectedPasswordItem).ParentGroup.UUID == _dbTree.MetaData.RecycleBinUUID;
            }
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
                if (_dbTree.MetaData.RecycleBinEnabled && !PasswordIsInRecycleBin)
                {
                    //Move the element to the recycle bin in the document
                    var recycleBinGroupElement = _dbTree.FindGroupByUuid(_dbTree.MetaData.RecycleBinUUID);
                    var clonedElement = new XElement(SelectedPasswordItem.Element);
                    recycleBinGroupElement.Add(clonedElement);

                    var recycleBinGroup = TopLevelGroups.FirstOrDefault(g => g.UUID == _dbTree.MetaData.RecycleBinUUID);
                    if (recycleBinGroup != null)
                    {
                        //Move the entry into the Recycle Bin group that's visible in the grid view
                        var clonedEntry = new PwEntry(clonedElement, (PwGroup)recycleBinGroup);
                        recycleBinGroup.SubGroupsAndEntries.Add(clonedEntry);
                    }
                }
                SelectedPasswordItem.Element.Remove();
                ((PwEntry)SelectedPasswordItem).Remove();
                await PWDatabaseDataSource.Instance.SavePwDatabase();
            }
        }

        public async void DeleteGroup()
        {
            if (Root.UUID == _dbTree.Group.UUID)
            {
                var rootMessage = String.Format("{0} is the top folder in your database, which you cannot delete.{1}Doing so would delete the entire database.{1}To delete your database simply delete the database file from your system.", Root.Name, Environment.NewLine);
                var rootDialog = new MessageDialog(rootMessage, "Can not delete database");
                await rootDialog.ShowAsync();
                return;
            }

            if (_dbTree.MetaData.RecycleBinEnabled && Root.UUID == _dbTree.MetaData.RecycleBinUUID)
            {
                var recycleMessage = String.Format("The folder you are trying to delete is set as your Recycle Bin.{0}To delete this folder either choose another folder as your Recycle Bin,{0}or disable the Recycle Bin setting in your database options.", Environment.NewLine);
                var recycleDialog = new MessageDialog(recycleMessage, "Can not delete recycle bin");
                await recycleDialog.ShowAsync();
                return;
            }
            
            var confirmMessage = String.Format("Are you sure you want to delete the {0} folder?{1}This will delete all of its contents too, which means all of the passwords and folders{1}you see now on the screen.", Root.Name, Environment.NewLine);
            var confirmDialog = new MessageDialog(confirmMessage, "Confirm Delete");
            bool result = false;

            confirmDialog.Commands.Add(new UICommand("Yes", (cmd) => result = true));
            confirmDialog.Commands.Add(new UICommand("No", (cmd) => result = false));
            confirmDialog.DefaultCommandIndex = 0;
            confirmDialog.CancelCommandIndex = 1;

            await confirmDialog.ShowAsync();

            if (result)
            {
                if (_dbTree.MetaData.RecycleBinEnabled)
                {
                    //Move the folder to the recycle bin in the document
                    var recycleBinGroupElement = _dbTree.FindGroupByUuid(_dbTree.MetaData.RecycleBinUUID);
                    var clonedElement = new XElement(Root.Element);
                    recycleBinGroupElement.Add(clonedElement);
                }
                Root.Element.Remove();
                await PWDatabaseDataSource.Instance.SavePwDatabase();
                _navigationService.GoBack();
            }
        }

        public void GoHome()
        {
            var dbRootUUID = WebUtility.UrlEncode(_dbTree.Group.UUID);
            _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, dbRootUUID).Navigate();
        }
    }
}