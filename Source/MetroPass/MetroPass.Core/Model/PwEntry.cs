using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public class PwEntry : PwCommon
    {
        private readonly PwGroup _parentGroup;

        public PwEntry(XElement element, PwGroup parentGroup)
        {
            _parentGroup = parentGroup;
            Element = element;

            Fields = Meta.ToDictionary(m => m.Element("Key").Value, m=> m.Element("Value").Value);     
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

        public IDictionary<string, string> Fields
        {
            get;
            set;
        }
    }
}