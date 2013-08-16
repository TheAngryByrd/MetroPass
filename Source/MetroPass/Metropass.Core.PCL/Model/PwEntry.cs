using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Metropass.Core.PCL.Model
{
    public class PwEntry : PwCommon
    {
        private readonly PwGroup _parentGroup;

        public PwEntry(XElement element, PwGroup parentGroup)
        {
            _parentGroup = parentGroup;
            Element = element;

            CustomFields = Meta.ToDictionary(m => m.Element("Key"), m=> m.Element("Value"));

            try
            {
                CustomFields.Remove(GetElementKey("title"));
                CustomFields.Remove(GetElementKey("username"));
                CustomFields.Remove(GetElementKey("password"));
                CustomFields.Remove(GetElementKey("url"));
                CustomFields.Remove(GetElementKey("notes"));
            }
            catch { }
        }

        public IEnumerable<XElement> Meta
        {
            get { return Element.Elements("String"); }
        }

        public PwGroup ParentGroup
        {
            get { return _parentGroup; }
        }

        public void Remove()
        {
            ParentGroup.SubGroupsAndEntries.Remove(this);
        }

        private XElement GetElement([CallerMemberName] String propertyName = null)
        {
            return Meta.FirstOrDefault(a => a.Element("Key").Value.ToLower() == propertyName.ToLower());
        }


        private XElement GetElementKey([CallerMemberName] String propertyName = null)
        {
            return GetElement(propertyName).Element("Key");
        }

        private XElement GetElementValue([CallerMemberName] String propertyName = null)
        {
            var foundElement = GetElement(propertyName);

            if(foundElement == null)
            {
                return new XElement(propertyName);
            }

            return foundElement.Element("Value");
        }
           
        public string Title
        {
            get { return GetElementValue().Value; }
            set { GetElementValue().Value = value; }
        }

        public string Username
        {
            get { return GetElementValue().Value; }
            set { GetElementValue().Value = value; }
        }

        public string Password
        {
            get { return GetElementValue().Value; }
            set { GetElementValue().Value = value; }
        }

        public string Url
        {
            get { return GetElementValue().Value; }
            set { GetElementValue().Value = value; }
        }

        public string Notes
        {
            get { return GetElementValue().Value; }
            set { GetElementValue().Value = value; }
        }

        public IDictionary<XElement, XElement> CustomFields
        {
            get;
            set;
        }
    }
}
