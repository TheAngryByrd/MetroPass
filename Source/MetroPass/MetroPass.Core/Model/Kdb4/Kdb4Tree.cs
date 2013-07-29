using MetroPass.Core.Exceptions;
using MetroPass.Core.Interfaces;
using System;
using System.Linq;
using System.Xml.Linq;
using Metropass.Core.PCL.Model;

namespace MetroPass.Core.Model.Kdb4
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

        public XElement FindEntryByUuid(string entryId)
        {
            var entryElements = Document.Descendants("Entry").Where(e => e.Element("UUID") != null && e.Element("UUID").Value == entryId);
            if (entryElements.Count() == 1)
            {
                return entryElements.Single();
            }
            if (entryElements.Count() > 0)
            {
                entryElements = entryElements.Where(e => e.Parent.Name != "History");
                if (entryElements.Count() == 1)
                {
                    return entryElements.Single();
                }
            }
            throw new ArgumentException(string.Format("Could not find Entry with ID {0} in the database.", entryId), entryId);
        }

        public XElement FindGroupByUuid(string groupId)
        {
            var groupElements = Document.Descendants("Group").Where(g => g.Element("UUID").Value == groupId);
            if (groupElements.Count() == 1)
            {
                return groupElements.Single();
            }
            throw new GroupNotFoundException(string.Format("Cound not find Group with ID {0} in the database.", groupId), groupId);
        }
    }
}