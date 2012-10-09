using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public class PwEntry : PwCommon
    {
        public PwEntry(XElement element)
        {
            Element = element;
        }

        public IEnumerable<XElement> Meta
        {
            get { return Element.Elements("String"); }
        }

        private XElement GetElement([CallerMemberName] String propertyName = null)
        {
            var foundElement = Meta.FirstOrDefault(a => a.Element("Key").Value.ToLower() == propertyName.ToLower());

            if(foundElement == null)
            {
                return new XElement(propertyName);
            }

            return foundElement.Element("Value");
        }
           
        public string Title
        {
            get { return GetElement().Value; }
            set { GetElement().Value = value; }
        }

        public string Username
        {
            get { return GetElement().Value; }
            set { GetElement().Value = value; }
        }

        public string Password
        {
            get { return GetElement().Value; }
            set { GetElement().Value = value; }
        }

        public string Url
        {
            get { return GetElement().Value; }
            set { GetElement().Value = value; }
        }

        public string Notes
        {
            get { return GetElement().Value; }
            set { GetElement().Value = value; }
        }
    }
}