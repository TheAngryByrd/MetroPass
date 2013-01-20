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

        public DatabaseSettingsViewModel(IKdbTree dbTree)
        {
            _dbTree = dbTree;
            FillGroups(dbTree.Group, 0);

            this.DisplayName = "Database Options";
        }

        private void FillGroups(PwGroup rootGroup, int level)
        {
            _availablRecycleBinGroups.Add(new PwGroupLevels { Group = rootGroup, Level = level });
            level++;
            foreach (var subGroup in rootGroup.SubGroups)
            {
                FillGroups(subGroup, level);
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

        public IEnumerable<PwGroupLevels> AvailableGroups
        {
            get
            {
                return _availablRecycleBinGroups;
            }
        }

        public PwGroupLevels SelectedRecycleBinGroup
        {
            get
            {
                return _availablRecycleBinGroups.FirstOrDefault(g => g.Group.UUID == _dbTree.MetaData.RecycleBinUUID);
            }
            set
            {
                if (_dbTree.MetaData.RecycleBinUUID != value.Group.UUID) {
                    _dbTree.MetaData.RecycleBinUUID = value.Group.UUID;
                    _dbTree.MetaData.RecycleBinChanged = DateTime.Now.ToFormattedUtcTime();
                    NotifyOfPropertyChange(() => SelectedRecycleBinGroup);
                }
            }
        }
    }

    public struct PwGroupLevels
    {
        public int Level { get; set; }
        public PwGroup Group { get; set; }
        public string DisplayName
        {
            get
            {
                return string.Format("{0}{1}", new string(' ', Level * 2), Group.Name);
            }
        }
    }
}