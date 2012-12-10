using System.Collections.ObjectModel;

namespace MetroPass.Core.Model
{
    public interface IGroup
    {
        string Name { get; set; }
        string UUID { get; }
        ObservableCollection<PwCommon> SubGroupsAndEntries { get; }
    }
}