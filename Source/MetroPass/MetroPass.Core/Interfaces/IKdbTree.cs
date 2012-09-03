using MetroPass.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbTree
    {
         XDocument Document { get;  set; }
         PwGroup Group { get; set; }
    }
}
