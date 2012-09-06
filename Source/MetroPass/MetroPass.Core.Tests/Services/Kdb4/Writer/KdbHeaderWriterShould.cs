using MetroPass.Core.Services.Kdb4.Writer;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using MetroPass.Core.Model;
namespace MetroPass.Core.Tests.Services.Kdb4.Writer
{
    [TestClass]
    public class KdbHeaderWriterShould
    {
        Kdb4HeaderWriter hw;
        MemoryStream s;
        IOutputStream outputStream;
        IDataWriter dataWriter;
        [TestInitialize]
        public void Init()
        {
             hw = new Kdb4HeaderWriter();
             s = new MemoryStream();
             outputStream = s.AsOutputStream();
             dataWriter = new DataWriter(outputStream);
        }

        [TestMethod]
        public void WriteAHeaderField()
        {
            var data = CryptographicBuffer.GenerateRandom(32);

            hw.WriteHeaderField(dataWriter, Model.Kdb4.Kdb4HeaderFieldID.MasterSeed, data);

            Assert.AreEqual(dataWriter.UnstoredBufferLength, (uint)33);
        }

        [TestMethod]
        public async Task WriteAllHeaders()
        {
            var database = new PwDatabase(null);
            await hw.WriteHeaders(dataWriter, database);
        }
    }
}
