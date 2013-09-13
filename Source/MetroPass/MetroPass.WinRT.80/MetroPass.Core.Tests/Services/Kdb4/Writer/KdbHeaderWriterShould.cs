using MetroPass.WinRT.Infrastructure;
using MetroPass.WinRT.Infrastructure.Compression;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.IO;
using System.Threading.Tasks;
using Windows.Security.Cryptography;

namespace MetroPass.Core.Tests.Services.Kdb4.Writer
{
    [TestClass]
    public class KdbHeaderWriterShould
    {
        Kdb4HeaderWriter hw;
        MemoryStream randomStream;

        BinaryWriter dataWriter;
        private const string PasswordDatabasePath = "Data\\Pass.kdbx";
        private const string PasswordDatabasePassword = "UniquePassword";
        [TestInitialize]
        public void Init()
        {
             hw = new Kdb4HeaderWriter();
             randomStream = new MemoryStream();

             dataWriter = new BinaryWriter(randomStream);
             

        }

        [TestMethod]
        public void WriteAHeaderField()
        {
            var data = CryptographicBuffer.GenerateRandom(32);

            hw.WriteHeaderField(dataWriter, Kdb4HeaderFieldID.MasterSeed, data.AsBytes());

            Assert.AreEqual(dataWriter.BaseStream.Length, (uint)35);
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
     

            hw.WriteHeaders(dataWriter, kdb4File);
            var stream = dataWriter.BaseStream;
            stream.Position = 0;
            var factory = new KdbReaderFactory(
                          new WinRTCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                          new MultiThreadedBouncyCastleCrypto(),
                          new SHA256HasherRT(),
                          new GZipFactoryRT());

      
            var headerinfo = factory.ReadVersionInfo(stream);
        }


    }
}
