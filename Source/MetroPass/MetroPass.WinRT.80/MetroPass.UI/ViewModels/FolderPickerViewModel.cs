using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using MetroPass.UI.DataModel;
using MetroPass.UI.ViewModels.Messages;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;

namespace MetroPass.UI.ViewModels
{
    public enum FolderPickerMode
    {
        Move,
        RecycleBin
    }

    public class FolderPickerViewModel : Screen
    {

        private readonly IEventAggregator _eventAggregator;
        
        private readonly IPWDatabaseDataSource _dataSource;

        public FolderPickerViewModel(IEventAggregator eventAggregator,
            IPWDatabaseDataSource dataSource)
        {
            _dataSource = dataSource;
            _eventAggregator = eventAggregator;

            Mode = FolderPickerMode.Move;
            FillGroups(_dataSource.PwDatabase.Tree.Group, 0);
        }

        public FolderPickerMode Mode { get; set; }

        private void FillGroups(PwGroup rootGroup, int level)
        {
            _availableGroups.Add(new PwGroupLevels { Group = rootGroup, Level = level });
            level++;
            foreach (var subGroup in rootGroup.SubGroups)
            {
                FillGroups(subGroup, level);
            }
        }

       private readonly List<PwGroupLevels> _availableGroups = new List<PwGroupLevels>();
        public IEnumerable<PwGroupLevels> AvailableGroups
        {
            get
            {
                return _availableGroups;
            }
        }

        public string _selectedUUId;
        public PwGroupLevels SelectedGroup
        {
            get
            {
                return _availableGroups.FirstOrDefault(g => g.Group.UUID == _selectedUUId);
            }
            set
            {
                if (_selectedUUId != value.Group.UUID)
                {
                    _selectedUUId = value.Group.UUID;

                    NotifyOfPropertyChange(() => SelectedGroup);
                    if (Mode == FolderPickerMode.Move)
                    {
                        _eventAggregator.Publish(new FolderSelectedMessage { FolderUUID = _selectedUUId });
                    } else {
                        _eventAggregator.Publish(new RecycleBinSelectedMessage { FolderUUID = _selectedUUId });
                    }
                }
            }
        }

        public string CurrentFolderUUID
        {
            get { return _selectedUUId; }
            set
            {
                _selectedUUId = value;
                NotifyOfPropertyChange(() => SelectedGroup);
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
