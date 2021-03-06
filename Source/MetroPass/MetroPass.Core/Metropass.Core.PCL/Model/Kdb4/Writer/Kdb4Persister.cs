﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Metropass.Core.PCL.Cipher;
using Metropass.Core.PCL.Model.Kdb4;
using System.Xml.Linq;

namespace Metropass.Core.PCL.Model.Kdb4.Writer
{
    public class Kdb4Persister
    {
        private readonly CryptoRandomStream cryptoRandomStream;

        public Kdb4Persister(CryptoRandomStream stream)
        {
            this.cryptoRandomStream = stream;
        }

        public byte[] Persist(IKdbTree tree, byte[] hashOfHeader)
        {
            var metaElement = tree.Document.Descendants("Meta").FirstOrDefault();
            if (metaElement == null)
            {
                metaElement = new XElement("Meta", new XElement("Generator", "MetroPass"));
                tree.Document.Add(metaElement);
            }
            var headerHashElement = metaElement.Elements("HeaderHash").FirstOrDefault();
            if (headerHashElement == null)
            {
                headerHashElement = new XElement("HeaderHash");
                metaElement.Add(headerHashElement);
            }
            headerHashElement.Value = Convert.ToBase64String(hashOfHeader);
            var root = tree.Document.Descendants("Root").First();

            EncodeXml(root);

            MemoryStream ms = new MemoryStream();

            using (XmlWriter xw = XmlWriter.Create(ms))
            {
                tree.Document.WriteTo(xw);
            }

            return ms.ToArray() ;         
        }

        public void EncodeXml(XElement root)
        {
            var attr = root.Attribute("Protected");
            if (attr != null && Convert.ToBoolean(attr.Value))
            {
                var protectedStringBytes = UTF8Encoding.UTF8.GetBytes(root.Value);
                int protectedByteLength = protectedStringBytes.Length;


                var cipher = cryptoRandomStream.GetRandomBytes((uint)protectedByteLength);
                byte[] pbPlain = new byte[protectedStringBytes.Length];


                for (int i = 0; i < pbPlain.Length; ++i)
                    pbPlain[i] = (byte)(protectedStringBytes[i] ^ cipher[i]);

                string mypass = Convert.ToBase64String(pbPlain);
                root.SetValue(mypass);
            }

            foreach (var node in root.Elements())
            {
                EncodeXml(node);
            }
        }

    }
}
