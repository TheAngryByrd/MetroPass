using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Framework;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;

namespace MetroPass.UI.ViewModels
{
    public class DatabaseSettingsViewModel : Screen
    {
        private readonly IKdbTree _dbTree;
        private readonly List<PwGroupLevels> _availablRecycleBinGroups = new List<PwGroupLevels>();

        public FolderPickerViewModel FolderPickerViewModel {get;set;}

        public DatabaseSettingsViewModel(IKdbTree dbTree, FolderPickerViewModel folderPicker)
        {
            this.FolderPickerViewModel = folderPicker;    
            _dbTree = dbTree;
            this.DisplayName = "Database Options";

            var recycleBin = FolderPickerViewModel.AvailableGroups.FirstOrDefault(g => g.Group.UUID == _dbTree.MetaData.RecycleBinUUID);
            if (recycleBin.Group != null)
            {
                FolderPickerViewModel.SelectedGroup = recycleBin;
            }
   
            this.FolderPickerViewModel.SelectedGroupChange += FolderPickerViewModel_SelectedGroupChange;
        }

        void FolderPickerViewModel_SelectedGroupChange(object sender, string e)
        {
            if (_dbTree.MetaData.RecycleBinUUID != e)
            {
                _dbTree.MetaData.RecycleBinUUID = e;
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