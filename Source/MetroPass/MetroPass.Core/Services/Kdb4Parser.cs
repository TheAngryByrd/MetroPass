using MetroPass.Core.Helpers.Cipher;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MetroPass.Core.Services
{
    public class Kdb4Parser
    {
        public CryptoRandomStream _cryptoStream { get; set; }
        public Kdb4Parser(CryptoRandomStream crypto)
        {
            _cryptoStream = crypto;
        }

        public Kdb4Tree Parse(Stream decrypredDatabase)
        {

            var xml = CreateXmlReader(decrypredDatabase);
            var root = xml.Descendants("Root").First();

            DecodeXml(root);

            var groups = root.Element("Group");

            var kdb4Tree = new Kdb4Tree(xml);
            kdb4Tree.Group = ParseGroup(groups);

            return kdb4Tree;
        }

        public ObservableCollection<PwGroup> ParseGroups(IEnumerable<XElement> elementGroups)
        {
            var groups = new ObservableCollection<PwGroup>();

            foreach (var element in elementGroups)
            {
                groups.Add(ParseGroup(element));
            }
            return groups;
        }

        public PwGroup ParseGroup(XElement elementGroup)
        {
            var group = new PwGroup(elementGroup);


            group.IconId = XmlConvert.ToInt16(elementGroup.Element("IconID").Value);


            group.Name = elementGroup.Element("Name").Value;

            var timeElement = elementGroup.Element("Times");

            group.CreationDate = DateTime.Parse(timeElement.Element("CreationTime").Value);
            group.LastModifiedDate = DateTime.Parse(timeElement.Element("LastModificationTime").Value);
            group.LastAccessTime = DateTime.Parse(timeElement.Element("LastAccessTime").Value);
            group.ExpireTime = DateTime.Parse(timeElement.Element("ExpiryTime").Value);

            var entryElements = elementGroup.Elements("Entry");

            foreach (var element in entryElements)
            {
                group.Entries.Add(ParseEntry(element));
            }

            group.SubGroups = ParseGroups(elementGroup.Elements("Group"));

            return group;
        }

        public PwEntry ParseEntry(XElement entryElement)
        {
            var entry = new PwEntry(entryElement);

            entry.IconId = XmlConvert.ToInt16(entryElement.Element("IconID").Value);

            var timeElement = entryElement.Element("Times");

            entry.CreationDate = DateTime.Parse(timeElement.Element("CreationTime").Value);
            entry.LastModifiedDate = DateTime.Parse(timeElement.Element("LastModificationTime").Value);
            entry.LastAccessTime = DateTime.Parse(timeElement.Element("LastAccessTime").Value);
            entry.ExpireTime = DateTime.Parse(timeElement.Element("ExpiryTime").Value);

            var meta = entryElement.Elements("String");

            foreach (var item in meta)
            {
                var key = item.Element("Key").Value;
                var value = item.Element("Value").Value;

                if (key == "Title")
                {
                    entry.Title = value;
                }
                else if (key == "Username")
                {
                    entry.Username = value;
                }
                else if (key == "Password")
                {
                    entry.Password = value;
                }
                else if (key == "URL")
                {
                    entry.Url = value;
                }
                else if (key == "Notes")
                {
                    entry.Notes = value;
                }

            }

            return entry;
        }

        public static XmlReaderSettings CreateStdXmlReaderSettings()
        {
            XmlReaderSettings xrs = new XmlReaderSettings();

            xrs.CloseInput = true;
            xrs.IgnoreComments = true;
            xrs.IgnoreProcessingInstructions = true;
            xrs.IgnoreWhitespace = true;


            return xrs;
        }

        public static XDocument CreateXmlReader(Stream readerStream)
        {

            XmlReaderSettings xrs = CreateStdXmlReaderSettings();

            return XDocument.Load(XmlReader.Create(readerStream, xrs));
        }

        public static void DecodeXml(XElement root)
        {
            var attr = root.Attribute("Protected");
            if (attr != null && Convert.ToBoolean(attr.Value))
            {

            }
        }


    }
}
