using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public class PwGroup : PwCommon
    {

        public XElement Element { get; private set; }
        public int IconId { get; set; }
        public string Name { get; set; }


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
