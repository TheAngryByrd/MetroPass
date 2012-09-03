using MetroPass.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetroPass.Core.Model.Kdb4
{
    public class Kdb4Tree : IKdbTree
    {
        public XDocument Document { get; private set; }
        public PwGroup Group { get; set; }
        public Kdb4Tree(XDocument document)
        {
            Document = document;
        }

       
    }
}
