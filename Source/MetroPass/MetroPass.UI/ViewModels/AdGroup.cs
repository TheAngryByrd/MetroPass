using System.Collections.ObjectModel;
using Metropass.Core.PCL.Model;

namespace MetroPass.UI.ViewModels
{
    public class AdGroup : IGroup
    {
        public string Name { get; set; }

        public AdGroup()
        {
            Name = "Advertisement";
        }

        public string UUID
        {
            get
            {
                return "";
            }
        }

        public ObservableCollection<PwCommon> SubGroupsAndEntries
        {
            get
            {
                return new ObservableCollection<PwCommon>() { new AdItem() };
            }
        }
    }
}