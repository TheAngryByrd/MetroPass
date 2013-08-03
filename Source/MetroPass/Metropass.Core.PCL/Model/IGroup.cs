using System.Collections.ObjectModel;
using Metropass.Core.PCL.Model;

namespace MetroPass.Core.Model
{
    public interface IGroup
    {
        string Name { get; set; }
        string UUID { get; }
        ObservableCollection<PwCommon> SubGroupsAndEntries { get; }
    }
}