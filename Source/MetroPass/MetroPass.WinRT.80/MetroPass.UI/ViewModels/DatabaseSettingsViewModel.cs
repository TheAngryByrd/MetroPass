using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.ViewModels.Messages;

namespace MetroPass.UI.ViewModels
{
    public class DatabaseSettingsViewModel : Screen, IHandle<RecycleBinSelectedMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly List<PwGroupLevels> _availablRecycleBinGroups = new List<PwGroupLevels>();
        private readonly IPWDatabaseDataSource _dataSource;

        public FolderPickerViewModel FolderPickerViewModel { get; set; }

        public DatabaseSettingsViewModel(
            IEventAggregator eventAggregator, 
            FolderPickerViewModel folderPicker,
            IPWDatabaseDataSource dataSource)
        {
            _dataSource = dataSource;
            _eventAggregator = eventAggregator;
            this.FolderPickerViewModel = folderPicker;
            this.DisplayName = "Database Options";

            folderPicker.Mode = FolderPickerMode.RecycleBin;
            var recycleBin = FolderPickerViewModel.AvailableGroups.FirstOrDefault(g => g.Group.UUID == _dataSource.PwDatabase.Tree.MetaData.RecycleBinUUID);
            if (recycleBin.Group != null)
            {
                FolderPickerViewModel.CurrentFolderUUID = recycleBin.Group.UUID;
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventAggregator.Unsubscribe(this);
        }

        public void Handle(RecycleBinSelectedMessage message)
        {
            var tree = _dataSource.PwDatabase.Tree;
            if (tree.MetaData.RecycleBinUUID != message.FolderUUID)
            {
                tree.MetaData.RecycleBinUUID = message.FolderUUID;
                tree.MetaData.RecycleBinChanged = DateTime.Now.ToFormattedUtcTime();
            }
        }

        public bool RecycleBinEnabled
        {

            get { return _dataSource.PwDatabase.Tree.MetaData.RecycleBinEnabled; }
            set
            {
                if (_dataSource.PwDatabase.Tree.MetaData.RecycleBinEnabled != value)
                {
                    _dataSource.PwDatabase.Tree.MetaData.RecycleBinEnabled = value;
                    NotifyOfPropertyChange(() => RecycleBinEnabled);
                }
            }
        } 
    }
}