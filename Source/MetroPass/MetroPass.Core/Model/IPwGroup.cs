using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public interface IGroup
    {
        string Name { get; set; }

        ObservableCollection<PwCommon> SubGroupsAndEntries { get; }
    }
}