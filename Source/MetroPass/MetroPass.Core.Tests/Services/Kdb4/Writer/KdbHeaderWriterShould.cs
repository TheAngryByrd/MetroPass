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
namespace MetroPass.Core.Tests.Services.Kdb4.Writer
{
    [TestClass]
    public class KdbHeaderWriterShould
    {
        [TestMethod]
        public void WriteAHeaderField()
        {
            Kdb4HeaderWriter hw = new Kdb4HeaderWriter();
            MemoryStream s = new MemoryStream();
            var outputStream = s.AsOutputStream();
            var dataWriter = new DataWriter(outputStream);
            var data = CryptographicBuffer.GenerateRandom(32);

            hw.WriterHeaderField(dataWriter, Model.Kdb4.Kdb4HeaderFieldID.MasterSeed, data);

            Assert.AreEqual(dataWriter.UnstoredBufferLength, (uint)33);
        }
    }
}
