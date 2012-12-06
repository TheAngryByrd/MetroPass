using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;

namespace MetroPass.UI.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private readonly IKdbTree _dbTree;
        private readonly IEnumerable<PwGroup> _availablRecycleBinGroups;

        public SettingsViewModel(IKdbTree dbTree)
        {
            _dbTree = dbTree;
            _availablRecycleBinGroups = dbTree.Group.SubGroups.ToList();

            this.DisplayName = "Options";
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

        public IEnumerable<PwGroup> AvailableGroups
        {
            get
            {
                return _availablRecycleBinGroups;
            }
        }

        public PwGroup SelectedRecycleBinGroup
        {
            get
            {
                return _availablRecycleBinGroups.FirstOrDefault(g => g.UUID == _dbTree.MetaData.RecycleBinUUID);
            }
            set
            {
                if (_dbTree.MetaData.RecycleBinUUID != value.UUID) {
                    _dbTree.MetaData.RecycleBinUUID = value.UUID;
                    NotifyOfPropertyChange(() => SelectedRecycleBinGroup);
                }
            }
        }
    }
}