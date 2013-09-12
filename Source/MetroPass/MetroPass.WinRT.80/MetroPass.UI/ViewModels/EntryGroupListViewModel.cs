using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Caliburn.Micro;
using Callisto.Controls;
using Framework;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.ViewModels.Messages;
using Metropass.Core.PCL.Exceptions;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace MetroPass.UI.ViewModels
{
    public class EntryGroupListViewModel : PasswordEntryScreen, IHandle<FolderSelectedMessage>
    {
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<IGroup> _topLevelGroups;
        private readonly IKdbTree _dbTree;
        
        public EntryGroupListViewModel(IKdbTree dbTree,
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IClipboard clipboard,
            IPageServices pageServices)
            : base(navigationService, eventAggregator, clipboard, pageServices)
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
                    Root = _dbTree.FindGroupByUuid(value);                    
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

        private Flyout FolderFlyout;
        public void MoveEntry(FrameworkElement source)
        {
            FolderFlyout = DialogService.ShowFlyout<FolderPickerViewModel>(PlacementMode.Top, source, SetupMoveEntry, onContentAdd: onContentAdd );
        }

        public async void Handle(FolderSelectedMessage message)
        {
            await MoveItem(message.FolderUUID, (PwEntry)SelectedPasswordItem);
            FolderFlyout.IsOpen = false;
            ShowAppBar = false;
        }

        private object onContentAdd(UIElement view)
        {
            var settingsColor = App.Current.Resources["MainAppColor"] as SolidColorBrush;


            Border content = new Border() { Child = view, BorderBrush = new SolidColorBrush(Colors.Black), Padding = (new Thickness(5,5,5,10)), BorderThickness = new Thickness(3) };
            return content;
        }

        public void SetupMoveEntry(FolderPickerViewModel fp)
        {
            fp.CurrentFolderUUID = ((PwEntry)SelectedPasswordItem).ParentGroup.UUID;
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
            if (
                await ConfirmDeletePassword() &&
                await VerifyRecylceBinAvailable()
            )
            {
                if (_dbTree.MetaData.RecycleBinEnabled && !PasswordIsInRecycleBin)
                {
                    //Move the element to the recycle bin in the document
                    var recycleBinGroupElement = _dbTree.FindGroupByUuid(_dbTree.MetaData.RecycleBinUUID);
                    var clonedElement = new XElement(SelectedPasswordItem.Element);
                    recycleBinGroupElement.AddEntryToDocument(new PwEntry((clonedElement),recycleBinGroupElement));

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

        public async Task MoveItem(string UUID, PwCommon selectedItem)
        {
            var folder = _dbTree.FindGroupByUuid(UUID);
            var clonedElement = new XElement(selectedItem.Element);
            folder.AddEntryToDocument(new PwEntry((clonedElement), folder));
            var UIFolder = TopLevelGroups.FirstOrDefault(g => g.UUID == UUID);
            if (UIFolder != null)
            {
                var clonedEntry = new PwEntry(clonedElement, (PwGroup)UIFolder);
                UIFolder.SubGroupsAndEntries.Add(clonedEntry);
     
            }
            selectedItem.Element.Remove();
            ((PwEntry)selectedItem).Remove();

            await PWDatabaseDataSource.Instance.SavePwDatabase();
        }

        public async void DeleteGroup()
        {
            if (
                await VerifyNotTopFolder() &&
                await VerifyNotDeletingRecycleBin() &&
                await VerifyRecylceBinAvailable() &&
                await ConfirmDeleteFolder())
            {
                if (_dbTree.MetaData.RecycleBinEnabled)
                {
                    //Move the folder to the recycle bin in the document
                    var recycleBinGroupElement = _dbTree.FindGroupByUuid(_dbTree.MetaData.RecycleBinUUID);
                    var clonedElement = new XElement(Root.Element);
                    recycleBinGroupElement.AddGroupToDocument(new PwGroup(clonedElement));
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

        private async Task<bool> VerifyNotTopFolder()
        {
            if (Root.UUID == _dbTree.Group.UUID)
            {
                var rootMessage = String.Format("{0} is the top folder in your database, which you cannot delete.{1}Doing so would delete the entire database.{1}To delete your database simply delete the database file from your system.", Root.Name, Environment.NewLine);
                var rootDialog = new MessageDialog(rootMessage, "Can not delete database");
                await rootDialog.ShowAsync();
                return false;
            }
            return true;
        }

        private async Task<bool> VerifyNotDeletingRecycleBin()
        {
            if (_dbTree.MetaData.RecycleBinEnabled && Root.UUID == _dbTree.MetaData.RecycleBinUUID)
            {
                var recycleMessage = String.Format("The folder you are trying to delete is set as your Recycle Bin.{0}To delete this folder either choose another folder as your Recycle Bin,{0}or disable the Recycle Bin setting in your database options.", Environment.NewLine);
                var recycleDialog = new MessageDialog(recycleMessage, "Can not delete recycle bin");
                await recycleDialog.ShowAsync();
                return false;
            }
            return true;
        }

        private async Task<bool> VerifyRecylceBinAvailable()
        {
            var recycleBinFound = true;

            if (_dbTree.MetaData.RecycleBinEnabled) {
                try
                {
                    var recycleBinGroupElement = _dbTree.FindGroupByUuid(_dbTree.MetaData.RecycleBinUUID);
                }
                catch (Exception)
                {
                    recycleBinFound = false;
                }
                if (!recycleBinFound) {
                    var message = String.Format("The Recycle Bin option is turned on in your database settings,{0}however MetroPass could not find the Recycle Bin folder.{0}Please check the settings and ensure a folder in your database is selected as the Recycle Bin.", Environment.NewLine);
                    var dialog = new MessageDialog(message, "Can not delete recycle bin");
                    await dialog.ShowAsync();
                }
            }

            return recycleBinFound;
        }

        private async Task<bool> ConfirmDeletePassword()
        {
            var confirmMessage = String.Format("Are you sure you want to delete the password for {0}?", ((PwEntry)SelectedPasswordItem).Title);
            var confirmDialog = new MessageDialog(confirmMessage, "Confirm Delete");
            bool result = false;

            confirmDialog.Commands.Add(new UICommand("Yes", (cmd) => result = true));
            confirmDialog.Commands.Add(new UICommand("No", (cmd) => result = false));
            confirmDialog.DefaultCommandIndex = 0;
            confirmDialog.CancelCommandIndex = 1;

            await confirmDialog.ShowAsync();
            return result;
        }
        private async Task<bool> ConfirmDeleteFolder()
        {
            var confirmMessage = String.Format("Are you sure you want to delete the {0} folder?{1}This will delete all of its contents too, which means all of the passwords and folders{1}you see now on the screen.", Root.Name, Environment.NewLine);
            var confirmDialog = new MessageDialog(confirmMessage, "Confirm Delete");
            bool result = false;

            confirmDialog.Commands.Add(new UICommand("Yes", (cmd) => result = true));
            confirmDialog.Commands.Add(new UICommand("No", (cmd) => result = false));
            confirmDialog.DefaultCommandIndex = 0;
            confirmDialog.CancelCommandIndex = 1;

            await confirmDialog.ShowAsync();
            return result;
        }
    }
}