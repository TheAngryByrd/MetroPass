using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public class PwEntry : PwCommon
    {
        public PwEntry(XElement element)
        {
            Element = element;
        }

        public XElement Element { get; set; }
        public int IconId { get; set; }



        public string Title { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string Notes { get; set; }
    }
}
