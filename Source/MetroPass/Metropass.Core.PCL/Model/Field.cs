using System.Xml.Linq;

namespace Metropass.Core.PCL.Model
{
    public class Field
    {
        public readonly XElement Element;

        public Field(XElement element)
        {
            Element = element;
        }

        private const string xml = "<String><Key>Name</Key><Value>Value</Value></String>";
        public static Field New()
        {
            return new Field(XElement.Parse(xml));
        }

        public string Name 
        {
            get
            {
                return Element.Element("Key").Value;
            }
            set
            {
                Element.Element("Key").Value = value;
            }
        }

        public bool Protected 
        {
            get
            {
                var element =Element.Element("Value").Attribute("Protected");
                if (element != null)
                {
                   return element.Value == "True";
                }
                return false;
            }
            set
            {
                Element.Element("Value").SetAttributeValue("Protected", value.ToString());
            }
        }

        public string Value 
        {
            get
            {
                return Element.Element("Value").Value;
            }
            set
            {
                Element.Element("Value").Value = value;
            } 
        }        
    }
}
