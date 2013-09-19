using System.Xml.Linq;

namespace Metropass.Core.PCL.Model.Kdb4
{
    public interface IKdbTree
    {
         XDocument Document { get;  set; }
         Kdb4TreeMetaData MetaData { get; set;}
         PwGroup Group { get; set; }
         PwEntry FindEntryByUuid(string entryId);
         PwGroup FindGroupByUuid(string groupId);

         PwGroup GetRecycleBin();
    }
}