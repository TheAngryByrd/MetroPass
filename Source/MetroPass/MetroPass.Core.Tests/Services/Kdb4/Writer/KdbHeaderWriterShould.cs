using Framework;
using MetroPass.Core.Services.Kdb4.Writer;
using Metropass.Core.PCL.Model.Kdb4;
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
using MetroPass.Core.Services;

namespace MetroPass.Core.Tests.Services.Kdb4.Writer
{
    [TestClass]
    public class KdbHeaderWriterShould
    {
        Kdb4HeaderWriter hw;
        InMemoryRandomAccessStream randomStream;

        IDataWriter dataWriter;
        private const string PasswordDatabasePath = "Data\\Pass.kdbx";
        private const string PasswordDatabasePassword = "UniquePassword";
        [TestInitialize]
        public void Init()
        {
             hw = new Kdb4HeaderWriter();
             randomStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();

             dataWriter = new DataWriter(randomStream);
             dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

        }

        [TestMethod]
        public void WriteAHeaderField()
        {
            var data = CryptographicBuffer.GenerateRandom(32);

            hw.WriteHeaderField(dataWriter, Metropass.Core.PCL.Model.Kdb4.Kdb4HeaderFieldID.MasterSeed, data);

            Assert.AreEqual(dataWriter.UnstoredBufferLength, (uint)35);
        }

        [TestMethod]
        public async Task WriteAllHeaders()
        {
            var database = (await Scenarios.LoadDatabase(PasswordDatabasePath, PasswordDatabasePassword, null));
            var kdb4File = new Kdb4File(database);
            kdb4File.pbMasterSeed = CryptographicBuffer.GenerateRandom(32).AsBytes();
            kdb4File.pbTransformSeed = CryptographicBuffer.GenerateRandom(32).AsBytes();
            kdb4File.pbEncryptionIV = CryptographicBuffer.GenerateRandom(16).AsBytes();
            kdb4File.pbProtectedStreamKey = CryptographicBuffer.GenerateRandom(32).AsBytes();
            kdb4File.pbStreamStartBytes = CryptographicBuffer.GenerateRandom(32).AsBytes();
     

            await hw.WriteHeaders(dataWriter, kdb4File);

           await dataWriter.StoreAsync();

           await dataWriter.FlushAsync();

            dataWriter.DetachStream();
            var data = randomStream.GetInputStreamAt(0);
            var reader = new DataReader(data);
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

            await reader.LoadAsync((uint)randomStream.Size);
            var factory = new KdbReaderFactory();
            var headerinfo = factory.ReadVersionInfo(reader.AsStream());
        }


    }
}
