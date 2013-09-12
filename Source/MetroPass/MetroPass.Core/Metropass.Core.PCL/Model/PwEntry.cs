using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Metropass.Core.PCL.Model
{
    public class PwEntry : PwCommon
    {
        private readonly PwGroup _parentGroup;

        private string[] knownFields = new[]
        {
            "Title","UserName","Password","URL","Notes"
        };

        private IDictionary<string, Field> _fields;

        public PwEntry(XElement element, PwGroup parentGroup)
        {
            _parentGroup = parentGroup;
            Element = element;

            SetupFields();       
        }
  
        private void SetupFields()
        {
            _fields = Meta.ToDictionary(m => m.Element("Key").Value, m => new Field(m));
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

        public IEnumerable<Field> CustomFields
        {
            get
            {
                return _fields.Keys.Except(knownFields).
                    Select(key => _fields[key]).
                    ToList();
            }
        }

        public void AddCustomFields(IEnumerable<Field> customFields)
        {
            var parents = Meta.Elements("Key").Where(e => !knownFields.Contains(e.Value)).Select(e => e.Parent);
            parents.Remove();
            foreach(var item in customFields)
            {
                Element.Add(item.Element);
            }
            SetupFields();
        }

        public static PwEntry New(PwGroup parent)     
        {
            var entryTemplate = @"
                <Entry>
                    <UUID>{0}</UUID>
                    <IconID>0</IconID>
                    <Times>
                        <LastModificationTime>{1}</LastModificationTime>
                        <CreationTime>{1}</CreationTime>
                        <LastAccessTime>{1}</LastAccessTime>
                        <ExpiryTime>{1}</ExpiryTime>
                        <LocationChanged>{1}</LocationChanged>
                        <Expires>False</Expires>
                        <UsageCount>0</UsageCount>
                    </Times>
                    <String>
                        <Key>Title</Key>
                        <Value>{2}</Value>
                    </String>
                    <String>
                        <Key>UserName</Key>
                        <Value>{3}</Value>
                    </String>
                    <String>
                        <Key>Password</Key>
                        <Value Protected=""True"">{4}</Value>
                    </String>
                    <String>
                        <Key>URL</Key>
                        <Value>{5}</Value>
                    </String>
                    <String>
                        <Key>Notes</Key>
                        <Value>{6}</Value>
                    </String>
                </Entry>
            ";
            var uuid = new PwUuid(true);

            entryTemplate = String.Format(entryTemplate, 
                Convert.ToBase64String(uuid.UuidBytes), 
                DateTime.Now.ToFormattedUtcTime(), 
                Sanitize("Title"), 
                Sanitize("Username"), 
                Sanitize("Password") ,
                Sanitize("Url") , 
                Sanitize("Notes"));

            var element = XElement.Parse(entryTemplate);
            var pwEntry = new PwEntry(element,parent);
            parent.AddEntryToDocument(pwEntry);
           
            return pwEntry;
        }

        private static string Sanitize(string text)
        {
            return WebUtility.HtmlEncode(WebUtility.HtmlDecode(text));
        }
    }
}
