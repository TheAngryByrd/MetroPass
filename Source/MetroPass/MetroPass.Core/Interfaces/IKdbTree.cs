using System.Xml.Linq;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;

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