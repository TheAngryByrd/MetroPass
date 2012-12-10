using System.Collections.ObjectModel;

namespace MetroPass.Core.Model
{
    public interface IGroup
    {
        string Name { get; set; }

        PwGroup Group { get; }

        ObservableCollection<PwCommon> SubGroupsAndEntries { get; }
    }
}