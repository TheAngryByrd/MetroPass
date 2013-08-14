﻿using System.Xml.Linq;

namespace Metropass.Core.PCL.Model.Kdb4
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