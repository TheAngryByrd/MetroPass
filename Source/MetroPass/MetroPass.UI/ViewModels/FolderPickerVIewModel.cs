using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;

namespace MetroPass.UI.ViewModels
{
    public class FolderPickerViewModel : Screen
    {
        private readonly IKdbTree dbTree;

        public event EventHandler<string> SelectedGroupChange;

        public FolderPickerViewModel(IKdbTree dbTree)
        {
            this.dbTree = dbTree;
            FillGroups(dbTree.Group, 0);

        }

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
                    if (SelectedGroupChange != null)
                    {
                        SelectedGroupChange(this, _selectedUUId);
                    }                   
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
