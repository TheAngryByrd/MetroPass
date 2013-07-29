using System.Xml.Linq;
using MetroPass.Core.Model.Kdb4;
using Metropass.Core.PCL.Model;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbTree
    {
         XDocument Document { get;  set; }
         Kdb4TreeMetaData MetaData { get; set;}
         PwGroup Group { get; set; }
         XElement FindEntryByUuid(string entryId);
         XElement FindGroupByUuid(string groupId);
    }
}