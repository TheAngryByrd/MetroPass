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

            var timeElement = elementGroup.Element("Times");
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
