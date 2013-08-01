using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Metropass.Core.PCL.Cipher;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;

namespace MetroPass.Core.Services
{
    public class Kdb4Parser
    {
        public CryptoRandomStream _cryptoStream { get; set; }

        public Kdb4Parser(CryptoRandomStream crypto)
        {
            _cryptoStream = crypto;
        }

        public Kdb4Tree ParseStream(Stream decrypredDatabase)
        {

            var xml = CreateXmlReader(decrypredDatabase);
            return ParseAndDecode(xml);
        }

        public Kdb4Tree ParseAndDecode(XDocument xml)
        {
            var root = xml.Descendants("Root").First();

            DecodeXml(root);

            return ParseXmlDocument(xml);
        }
        

        public Kdb4Tree ParseXmlDocument(XDocument xml)
        {
            var root = xml.Descendants("Root").First();

            var rootGroup = root.Element("Group");

            var kdb4Tree = new Kdb4Tree(xml);
            kdb4Tree.Group = ParseGroup(rootGroup);

            return kdb4Tree;
        }

        public PwGroup ParseGroup(XElement rootGroup)
        {
            var group = new PwGroup(rootGroup);
            return group;
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

        public void DecodeXml(XElement root)
        {

            var attr = root.Attribute("Protected");
            if (attr != null && Convert.ToBoolean(attr.Value))
            {
                var protectedStringBytes = Convert.FromBase64String(root.Value);
                int protectedByteLength = protectedStringBytes.Length;
    
           
                var cipher = _cryptoStream.GetRandomBytes((uint)protectedByteLength);
                byte[] pbPlain = new byte[protectedStringBytes.Length];


                for (int i = 0; i < pbPlain.Length; ++i)
                    pbPlain[i] = (byte)(protectedStringBytes[i] ^ cipher[i]);
                
                string mypass = UTF8Encoding.UTF8.GetString(pbPlain, 0, pbPlain.Length);
                root.SetValue(mypass);
            }

            foreach (var node in  root.Elements())
            {
                DecodeXml(node);
            }
        }


    }
}
