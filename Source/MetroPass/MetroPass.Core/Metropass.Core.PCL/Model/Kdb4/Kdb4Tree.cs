using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Metropass.Core.PCL.Exceptions;

namespace Metropass.Core.PCL.Model.Kdb4
{
    public class Kdb4Tree : IKdbTree
    {
        public XDocument Document { get;  set; }
        public PwGroup Group { get; set; }
        public Kdb4Tree(XDocument document)
        {
            Document = document;
            MetaData = new Kdb4TreeMetaData(document);
        }

        public Kdb4TreeMetaData MetaData { get; set; }

        public PwGroup GetRecycleBin()
        {
            PwGroup retVal = null;
            bool recycleBinExists = true;
            try
            {
                retVal = FindGroupByUuid(MetaData.RecycleBinUUID);
            }
           catch(Exception)
            {
                recycleBinExists = false;
            }

            if (recycleBinExists)
                return retVal;

            retVal = PwGroup.GetNewGroupElement();
            retVal.Name = "Recycle Bin";

            Group.AddGroupToDocument(retVal);            
            MetaData.RecycleBinUUID = retVal.UUID;

            return retVal;
            
        }

        public PwEntry FindEntryByUuid(string entryId)
        {
            var source = new List<PwGroup>() { Group };
            var result = source.All(g => g.SubGroups).
                SelectMany(g => g.Entries).
                SingleOrDefault(e => e.UUID == entryId);

            if (result != null)
            {
                return result;
            }           
          
            throw new ArgumentException(string.Format("Could not find Entry with ID {0} in the database.", entryId), entryId);
        }

        public PwGroup FindGroupByUuid(string groupId)
        {
            var source = new List<PwGroup>() { Group };
            var result = source.FindAll(g => g.SubGroups, g => g.UUID == groupId);

            if (result != null)
            {
                return result;
            }
            throw new GroupNotFoundException(string.Format("Cound not find Group with ID {0} in the database.", groupId), groupId);
        }
    }
}