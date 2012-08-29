using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MetroPassLib.Kdb4
{
    public class Kdb4Tree
    {
        public XDocument Document { get; private set; }

        public Kdb4Tree(XDocument document)
        {
            Document = document;
        }

        public PwGroup Group { get; set; }
    }
}
