using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Metropass.Core.PCL.Cipher;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using MetroPass.W81.Integration.Tests.Helpers;
using MetroPass.WinRT.Infrastructure;
using MetroPass.WinRT.Infrastructure.Compression;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PCLStorage;

namespace MetroPass.W81.Integration.Tests.Services
{
    [TestClass]
    public class PasswordOnlyDatabase
    {

        private const string PasswordDatabasePath = "Data\\Pass.kdbx";
        private const string PasswordDatabasePassword = "UniquePassword";
        [TestInitialize]
        public async Task Init()
        {


        }

        [TestMethod]
        public async Task CanOpenPasswordOnlyDatabase()
        {
            var kdb4Tree = (await Scenarios.LoadDatabase(PasswordDatabasePath, PasswordDatabasePassword, null)).Tree;
            var firstGroup = kdb4Tree.Group.SubGroups.FirstOrDefault(a => a.Name == "General");
            // Assert.AreEqual("General", firstGroup.Name);
            Assert.AreEqual(new DateTime(2012, 08, 25), firstGroup.LastAccessTime.Date);
            var firstEntry = kdb4Tree.Group.Entries.First();
            Assert.AreEqual("Notes", firstEntry.Notes);
            Assert.AreEqual("User Name", firstEntry.Username);
            Assert.AreEqual("Sample Entry", firstEntry.Title);
        }

        [TestMethod]
        public async Task CantOpenWithBadPassword()
        {
            AssertEx.ThrowsException<SecurityException>(() => Scenarios.LoadDatabase(PasswordDatabasePath, "NotPassword", null));

        }

        [TestMethod]
        public async Task CanWrite()
        {
            var database = await Scenarios.LoadDatabase(PasswordDatabasePath, PasswordDatabasePassword, null);
            var writer = new Kdb4Writer(new Kdb4HeaderWriter(),
                      new WinRTCrypto(),
                      new MultiThreadedBouncyCastleCrypto(),
                      new SHA256HasherRT(),
                      new GZipFactoryRT());
            //   .database.var file = await Package.Current.InstalledLocation.GetFileAsync(PasswordDatabasePath);

            try
            {

                var temp = await KnownFolders.DocumentsLibrary.GetFileAsync("file.kdbx");
                await temp.DeleteAsync();
            }
            catch (Exception e)
            {

            }

            var file = await KnownFolders.DocumentsLibrary.CreateFileAsync("file.kdbx");



            await writer.Write(database, new WinRTFile(file));

            await Scenarios.LoadDatabase(file, PasswordDatabasePassword, null);
        }


        [TestMethod]
        public async Task EncryptDecrypt()
        {
            var source = CryptographicBuffer.GenerateRandom(1137);
            var aesKey = CryptographicBuffer.GenerateRandom(32);
            var iv = CryptographicBuffer.GenerateRandom(16);
            var encrypted = EncryptDatabase(source, aesKey, iv);

            Assert.AreNotEqual(source, encrypted);

            var decryped = DecryptDatabase(encrypted, aesKey, iv);
            CollectionAssert.AreEqual(source.AsBytes(), decryped.AsBytes());

        }


        public IBuffer EncryptDatabase(IBuffer source, IBuffer aesKey, IBuffer iv)
        {
            var symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var aesCryptoKey = symKeyProvider.CreateSymmetricKey(aesKey);
            return CryptographicEngine.Encrypt(aesCryptoKey, source, iv);
        }

        public IBuffer DecryptDatabase(IBuffer source, IBuffer aesKey, IBuffer iv)
        {
            var symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var aesCryptoKey = symKeyProvider.CreateSymmetricKey(aesKey);
            return CryptographicEngine.Decrypt(aesCryptoKey, source, iv);
        }


        [TestMethod]
        public async Task HashedBlockStream()
        {

            var data = CryptographicBuffer.GenerateRandom(1024 * 1024).AsBytes();
            MemoryStream outStream = new System.IO.MemoryStream();
            var hashedBlockStream = new HashedBlockStream(outStream, true, new SHA256HasherRT());
            for (int i = 0; i < 1024 * 1024; i += 1024)
            {
                hashedBlockStream.Write(data, i, 1024);
            }
            hashedBlockStream.Flush();
            hashedBlockStream.Dispose();
            var fuckstick = outStream.ToArray();
        }

        [TestMethod]
        public async Task CompressEncryptDecrypDeCompress()
        {
            var mStream = CryptographicBuffer.GenerateRandom(1024 * 1024).AsBytes();

            MemoryStream outStream = new System.IO.MemoryStream();
            var tinyStream = ConfigureStream(outStream, PwCompressionAlgorithm.GZip, true);
            await tinyStream.WriteAsync(mStream, 0, mStream.Length);
            tinyStream.Dispose();
            byte[] bb = outStream.ToArray();
            var source = bb.AsBuffer();
            var aesKey = CryptographicBuffer.GenerateRandom(32);
            var iv = CryptographicBuffer.GenerateRandom(16);
            var encrypted = EncryptDatabase(source, aesKey, iv);

            CollectionAssert.AreNotEqual(mStream, encrypted.AsBytes());

            var decryped = DecryptDatabase(encrypted, aesKey, iv);

            Stream bigStream = ConfigureStream(new MemoryStream(decryped.AsBytes()), PwCompressionAlgorithm.GZip, false);
            System.IO.MemoryStream bigStreamOut = new System.IO.MemoryStream();
            bigStream.CopyTo(bigStreamOut);

            CollectionAssert.AreEqual(mStream, bigStreamOut.ToArray());

        }


        [TestMethod]
        public async Task CompressUncompress()
        {
            var mStream = CryptographicBuffer.GenerateRandom(1024).AsBytes();
            MemoryStream outStream = new System.IO.MemoryStream();
            var tinyStream = ConfigureStream(outStream, PwCompressionAlgorithm.GZip, true);

            await tinyStream.WriteAsync(mStream, 0, mStream.Length);

            tinyStream.Dispose();

            byte[] bb = outStream.ToArray();

            //Decompress                
            Stream bigStream = ConfigureStream(new MemoryStream(bb), PwCompressionAlgorithm.GZip, false);
            System.IO.MemoryStream bigStreamOut = new System.IO.MemoryStream();
            bigStream.CopyTo(bigStreamOut);

            CollectionAssert.AreEqual(mStream, bigStreamOut.ToArray());

        }

        public Stream ConfigureStream(Stream source, PwCompressionAlgorithm compression, bool IsWritingStream)
        {
            Stream hashedBlockStream = source;

            if (IsWritingStream)
            {
                hashedBlockStream = new HashedBlockStream(source, true, new SHA256HasherRT());

                if (compression == PwCompressionAlgorithm.GZip)
                {
                    hashedBlockStream = new GZipStream(hashedBlockStream, CompressionMode.Compress);
                }
            }
            else
            {
                hashedBlockStream = new HashedBlockStream(source, false, 0, false, new SHA256HasherRT());

                if (compression == PwCompressionAlgorithm.GZip)
                {
                    hashedBlockStream = new GZipStream(hashedBlockStream, CompressionMode.Decompress);
                }
            }

            return hashedBlockStream;
        }

        [TestMethod]
        public async Task UsingDataWriter()
        {
            var inMem = new InMemoryRandomAccessStream();

            var datawriter = new DataWriter(inMem);
            var starterBytes = CryptographicBuffer.GenerateRandom(32).AsBytes();
            var mStream = CryptographicBuffer.GenerateRandom(1024 * 1024).AsBytes();

            MemoryStream outStream = new System.IO.MemoryStream();
            var tinyStream = ConfigureStream(outStream, PwCompressionAlgorithm.GZip, true);
            await tinyStream.WriteAsync(starterBytes, 0, starterBytes.Length);
            await tinyStream.WriteAsync(mStream, 0, mStream.Length);
            tinyStream.Dispose();
            byte[] bb = outStream.ToArray();
            var source = bb.AsBuffer();
            var aesKey = CryptographicBuffer.GenerateRandom(32);
            var iv = CryptographicBuffer.GenerateRandom(16);
            var encrypted = EncryptDatabase(source, aesKey, iv);

            CollectionAssert.AreNotEqual(mStream, encrypted.AsBytes());

            datawriter.WriteBuffer(encrypted);
            await datawriter.StoreAsync();

            datawriter.DetachStream();
            datawriter.Dispose();
            var i = inMem.GetInputStreamAt(0);
            var datareader = new DataReader(i);
            datareader.ByteOrder = ByteOrder.LittleEndian;
            await datareader.LoadAsync((uint)inMem.Size);
            var fromStrea = datareader.ReadBuffer(datareader.UnconsumedBufferLength);

            var decryped = DecryptDatabase(fromStrea, aesKey, iv);

            Stream bigStream = ConfigureStream(new MemoryStream(decryped.AsBytes()), PwCompressionAlgorithm.GZip, false);
            System.IO.MemoryStream bigStreamOut = new System.IO.MemoryStream();
            bigStream.CopyTo(bigStreamOut);

            var buffer1 = CryptographicBuffer.CreateFromByteArray(bigStreamOut.ToArray());
            var reader = DataReader.FromBuffer(buffer1);
            var x = reader.ReadBuffer(32).AsBytes();
            CollectionAssert.AreEqual(starterBytes, x);
        }
        [TestMethod]
        public async Task TestProtectedDecrypt()
        {

            var protectedString = "QwzFTMLCpNY=";
            var protectedStringBytes = Convert.FromBase64String(protectedString);

            var rando = new CryptoRandomStream(CrsAlgorithm.Salsa20, Convert.FromBase64String("6tDlwZfwES4jAQzLisWdpNdnuTYyDZfflEdbshzdgi8="), new SHA256HasherRT());
            var getByte = rando.GetRandomBytes((uint)protectedStringBytes.Length);
            byte[] pbPlain = new byte[protectedStringBytes.Length];


            for (int i = 0; i < pbPlain.Length; ++i)
                pbPlain[i] = (byte)(protectedStringBytes[i] ^ getByte[i]);

            string mypass = UTF8Encoding.UTF8.GetString(pbPlain, 0, pbPlain.Length);

            Assert.AreEqual("Password", mypass);
        }






    }

}
