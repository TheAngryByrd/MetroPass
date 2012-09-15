using MetroPass.Core.Services.Kdb4.Writer;
using MetroPass.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.IO.Compression;
using Windows.Security.Cryptography.Core;
using MetroPass.Core.Model;
using MetroPass.Core.Security;

namespace MetroPass.Core.Tests.Services
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
            var firstGroup = kdb4Tree.Group.SubGroups.First();
            Assert.AreEqual("General", firstGroup.Name);
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
            var writer = new Kdb4Writer(new Kdb4HeaderWriter());
         //   .database.var file = await Package.Current.InstalledLocation.GetFileAsync(PasswordDatabasePath);

            try
            {

                var temp = await KnownFolders.DocumentsLibrary.GetFileAsync("file.kdbx");
                await temp.DeleteAsync();
            }catch(Exception e)
            {

            }

            var file = await KnownFolders.DocumentsLibrary.CreateFileAsync("file.kdbx");



            await writer.Write(database, file);

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
            
            var data = CryptographicBuffer.GenerateRandom(1024*1024).AsBytes();
            MemoryStream outStream = new System.IO.MemoryStream();
            var hashedBlockStream = new HashedBlockStream(outStream, true);
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
            var mStream = CryptographicBuffer.GenerateRandom(1024*1024).AsBytes();
       
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
            var tinyStream =ConfigureStream(outStream,PwCompressionAlgorithm.GZip,true);
            
            await tinyStream.WriteAsync(mStream, 0, mStream.Length);
               
            tinyStream.Dispose();

            byte[] bb = outStream.ToArray();

            //Decompress                
            Stream bigStream = ConfigureStream (new MemoryStream(bb), PwCompressionAlgorithm.GZip, false);
            System.IO.MemoryStream bigStreamOut = new System.IO.MemoryStream();
            bigStream.CopyTo(bigStreamOut);

            CollectionAssert.AreEqual(mStream, bigStreamOut.ToArray());

        }

        public Stream ConfigureStream(Stream source, PwCompressionAlgorithm compression, bool IsWritingStream)
        {
            Stream hashedBlockStream = source;

            if (IsWritingStream)
            {
                hashedBlockStream = new HashedBlockStream(source, true);

                if (compression == PwCompressionAlgorithm.GZip)
                {
                    hashedBlockStream = new GZipStream(hashedBlockStream, CompressionMode.Compress);
                }
            }
            else
            {
                hashedBlockStream = new HashedBlockStream(source, false, 0, false);

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







    }
    public sealed class HashingStreamEx : Stream
    {
        public Stream m_sBaseStream;
        private bool m_bWriting;
        private HashAlgorithmProvider m_hash;

        private byte[] m_pbFinalHash = null;

        public byte[] Hash
        {
            get { return m_pbFinalHash; }
        }

        public override bool CanRead
        {
            get { return !m_bWriting; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return m_bWriting; }
        }

        public override long Length
        {
            get { return m_sBaseStream.Length; }
        }

        public override long Position
        {
            get { return m_sBaseStream.Position; }
            set { throw new NotSupportedException(); }
        }

        public HashingStreamEx(Stream sBaseStream, bool bWriting, HashAlgorithmProvider hashAlgorithm)
        {
            if (sBaseStream == null) throw new ArgumentNullException("sBaseStream");

            m_sBaseStream = sBaseStream;
            m_bWriting = bWriting;


            m_hash = (hashAlgorithm ?? HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256));

     
        }

        public override void Flush()
        {
            m_sBaseStream.Flush();
        }

        //public override void Close()
        //{
        //    if (m_hash != null)
        //    {
        //        try
        //        {
        //            m_hash.TransformFinalBlock(new byte[0], 0, 0);

        //            m_pbFinalHash = m_hash.Hash;
        //        }
        //        catch (Exception) { Debug.Assert(false); }

        //        m_hash = null;
        //    }

        //    m_sBaseStream.Close();
        //}

        public override long Seek(long lOffset, SeekOrigin soOrigin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long lValue)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] pbBuffer, int nOffset, int nCount)
        {
            if (m_bWriting) throw new InvalidOperationException();

            int nRead = m_sBaseStream.Read(pbBuffer, nOffset, nCount);

            // Mono bug workaround (LaunchPad 798910)
            int nPartialRead = nRead;
            while ((nRead < nCount) && (nPartialRead != 0))
            {
                nPartialRead = m_sBaseStream.Read(pbBuffer, nOffset + nRead,
                    nCount - nRead);
                nRead += nPartialRead;
            }



            if ((m_hash != null) && (nRead > 0))
            {
                var hasher = m_hash.CreateHash();
                var mst = new MemoryStream(pbBuffer);
                var temp = new byte[0];
                mst.Read(temp,nOffset,nRead);
                hasher.Append(temp.AsBuffer());
                var hashedTemp = hasher.GetValueAndReset().AsBytes();
                mst.Write(hashedTemp, nOffset, nRead);
            }
             


            return nRead;
        }

        public override void Write(byte[] pbBuffer, int nOffset, int nCount)
        {
            if (!m_bWriting) throw new InvalidOperationException();



            if ((m_hash != null) && (nCount > 0))
            {
                var hasher = m_hash.CreateHash();
                var mst = new MemoryStream(pbBuffer);
                var temp = new byte[0];
                mst.Read(temp, nOffset, nCount);
                hasher.Append(temp.AsBuffer());
                var hashedTemp = hasher.GetValueAndReset().AsBytes();
                mst.Write(hashedTemp, nOffset, nCount);
            }
               // m_hash.TransformBlock(pbBuffer, nOffset, nCount, pbBuffer, nOffset);



            m_sBaseStream.Write(pbBuffer, nOffset, nCount);
        }
    }
}
