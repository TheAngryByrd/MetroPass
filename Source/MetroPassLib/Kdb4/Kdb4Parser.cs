using MetroPassLib.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;

namespace MetroPassLib.Kdb4
{
    public class Kdb4Parser
    {
        public CryptoRandomStream _cryptoStream { get; set; }
        public Kdb4Parser(CryptoRandomStream crypto)
        {
            _cryptoStream = crypto;
        }

        public Kdb4Tree Parse(IDataReader decrypredDatabase)
        {
            
            throw new NotImplementedException();
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
    }
}
