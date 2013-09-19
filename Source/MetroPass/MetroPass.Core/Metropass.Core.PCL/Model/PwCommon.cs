using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Metropass.Core.PCL.Model
{
    public abstract class PwCommon
    {
        public XElement Element { get; protected set; }

        public string UUID
        {
            get { return Element.Elements("UUID").First().Value; }
        }

        public int IconID
        {
            get { return XmlConvert.ToInt16(Element.Element().Value); }
            set { Element.Element().Value = value.ToString(); }
        }

        private XElement Times
        {
            get { return Element.Element(); }
        }


        public DateTime CreationTime
        {
            get { return DateTime.Parse(Times.Element().Value); }
            set { Times.Element().Value = value.ToString("yyyy-MM-ddTHH:mm:ssZ"); }
        }


        public DateTime LastModificationTime
        {
            get { return DateTime.Parse(Times.Element().Value); }
            set { Times.Element().Value = value.ToString("yyyy-MM-ddTHH:mm:ssZ"); }
        }


        public DateTime LastAccessTime
        {
            get { return DateTime.Parse(Times.Element().Value); }
            set { Times.Element().Value = value.ToString("yyyy-MM-ddTHH:mm:ssZ"); }
        }


        public DateTime ExpiryTime
        {
            get { return DateTime.Parse(Times.Element().Value); }
            set { Times.Element().Value = value.ToString("yyyy-MM-ddTHH:mm:ssZ"); }
        }


    }
}