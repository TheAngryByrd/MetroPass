using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetroPassLib
{
    public class PwGroup
    {
      
        public XElement Element { get; private set; }
        public int IconId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime ExpireTime { get; set; }

        public List<PwEntry> Entries { get; set; }
        public List<PwGroup> SubGroups { get; set; }

        public PwGroup(XElement element)
        {
            Element = element;
            Entries = new List<PwEntry>();
            SubGroups = new List<PwGroup>();
        }
    }
}
