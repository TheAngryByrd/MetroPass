using MetroPass.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MetroPass.Core.Interfaces
{
    public class IKdbTree
    {
        public XDocument Document { get; private set; }
        public PwGroup Group { get; set; }
    }
}
