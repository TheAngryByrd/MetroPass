using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage.Streams;
using MetroPass.Core.Interfaces;

namespace MetroPass.Core.Services.Kdb4.Writer
{
    public class Kdb4Persister
    {
        public IBuffer Persist(IKdbTree tree)
        {
            MemoryStream ms = new MemoryStream();

            using (XmlWriter xw = XmlWriter.Create(ms))
            {
                tree.Document.WriteTo(xw);
            }

            return ms.ToArray().AsBuffer() ;         
        }

    }
}
