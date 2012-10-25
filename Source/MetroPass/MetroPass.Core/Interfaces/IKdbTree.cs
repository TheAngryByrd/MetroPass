using MetroPass.Core.Model;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbTree
    {
         XDocument Document { get;  set; }
         PwGroup Group { get; set; }
         XElement FindEntryByUuid(string entryId);
         XElement FindGroupByUuid(string groupId);
    }
}