using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Framework;
using MetroPass.Core.Interfaces;
using MetroPass.UI.ViewModels.Messages;

namespace MetroPass.UI.ViewModels
{
    public class DatabaseSettingsViewModel : Screen, IHandle<RecycleBinSelectedMessage>
    {
        private readonly IKdbTree _dbTree;
        private readonly IEventAggregator _eventAggregator;
        private readonly List<PwGroupLevels> _availablRecycleBinGroups = new List<PwGroupLevels>();
        public FolderPickerViewModel FolderPickerViewModel { get; set; }

        public DatabaseSettingsViewModel(IKdbTree dbTree, IEventAggregator eventAggregator, FolderPickerViewModel folderPicker)
        {
            _eventAggregator = eventAggregator;
            _dbTree = dbTree;
            this.FolderPickerViewModel = folderPicker;
            this.DisplayName = "Database Options";

            folderPicker.Mode = FolderPickerMode.RecycleBin;
            var recycleBin = FolderPickerViewModel.AvailableGroups.FirstOrDefault(g => g.Group.UUID == _dbTree.MetaData.RecycleBinUUID);
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
            if (_dbTree.MetaData.RecycleBinUUID != message.FolderUUID)
            {
                _dbTree.MetaData.RecycleBinUUID = message.FolderUUID;
                _dbTree.MetaData.RecycleBinChanged = DateTime.Now.ToFormattedUtcTime();
            }
        }

        public bool RecycleBinEnabled
        {
            get { return _dbTree.MetaData.RecycleBinEnabled; }
            set
            {
                if (_dbTree.MetaData.RecycleBinEnabled != value)
                {
                    _dbTree.MetaData.RecycleBinEnabled = value;
                    NotifyOfPropertyChange(() => RecycleBinEnabled);
                }
            }
        } 
    }
}