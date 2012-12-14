using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Framework;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;

namespace MetroPass.UI.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private readonly IKdbTree _dbTree;
        private readonly List<PwGroupLevels> _availablRecycleBinGroups = new List<PwGroupLevels>();

        public SettingsViewModel(IKdbTree dbTree)
        {
            _dbTree = dbTree;
            FillGroups(dbTree.Group, 0);

            this.DisplayName = "Options";
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
        public const int MinMinutesToLockDatabase = 1;
        public bool LockDatabaseAfterInactivityEnabled
        {
            get
            {

                return SettingsModel.Instance.LockDatabaseAfterInactivityEnabled;
            }
            set
            {
                SettingsModel.Instance.MinutesToLockDatabase = value ? MinMinutesToLockDatabase : 0;

                NotifyOfPropertyChange(() => LockDatabaseAfterInactivityEnabled);
                NotifyOfPropertyChange(() => MinutesToLockDatabase);
            }
        }

        public int MinutesToLockDatabase
        {
            get
            {
                return SettingsModel.Instance.MinutesToLockDatabase;
            }
            set
            {
                SettingsModel.Instance.MinutesToLockDatabase = value;
                NotifyOfPropertyChange(() => MinutesToLockDatabase);
            }
        }

        public const int MinClearClipboardSeconds = 10;

        public bool SecondsToClearClipboardEnabled
        {
            get
            {
                return SettingsModel.Instance.ClearClipboardEnabled;
            }
            set
            {
                SettingsModel.Instance.SecondsToClearClipboard = value ? MinClearClipboardSeconds : 0;

                
                NotifyOfPropertyChange(() => SecondsToClearClipboardEnabled);
                NotifyOfPropertyChange(() => SecondsToClearClipboard);
            }
        }

        public int SecondsToClearClipboard
        {
            get
            {
                return SettingsModel.Instance.SecondsToClearClipboard;
            }
            set
            {
                SettingsModel.Instance.SecondsToClearClipboard = value;
                NotifyOfPropertyChange(() => SecondsToClearClipboard);
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