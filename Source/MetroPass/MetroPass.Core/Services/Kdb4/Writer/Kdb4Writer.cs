using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Framework;
using MetroPass.Core.Helpers.Cipher;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Security;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Model;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MetroPass.Core.Services.Kdb4.Writer
{
    public class Kdb4Writer : IKdbWriter
    {
        public Kdb4File kdb4File;
        private Kdb4HeaderWriter _headerWriter;
        public Kdb4Writer(Kdb4HeaderWriter headerWriter)
        {
            _headerWriter = headerWriter;
        }
        public async Task Write(PwDatabase databaseData, Windows.Storage.IStorageFile databaseFile)
        {
             kdb4File = new Kdb4File(databaseData);
            kdb4File.pbMasterSeed = CryptographicBuffer.GenerateRandom(32);
            kdb4File.pbTransformSeed = CryptographicBuffer.GenerateRandom(32);
            kdb4File.pbEncryptionIV = CryptographicBuffer.GenerateRandom(16);
            kdb4File.pbProtectedStreamKey = CryptographicBuffer.GenerateRandom(32);
            kdb4File.pbStreamStartBytes = CryptographicBuffer.GenerateRandom(32);
            
            
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            var datawriter = new DataWriter(memoryStream);
            datawriter.ByteOrder = ByteOrder.LittleEndian;

           await _headerWriter.WriteHeaders(datawriter, kdb4File);



           var header = datawriter.DetachBuffer();
           var hashOfHeader = SHA256Hasher.Hash( header);
            
           datawriter = new DataWriter();
           datawriter.WriteBuffer(header);

            var outStream = new MemoryStream();
            await outStream.WriteAsync(kdb4File.pbStreamStartBytes.AsBytes(), 0, (int)kdb4File.pbStreamStartBytes.Length);
            var configuredStream = ConfigureStream(outStream);

            var data = new Kdb4Persister(new CryptoRandomStream(CrsAlgorithm.Salsa20, kdb4File.pbProtectedStreamKey.AsBytes(), new SHA256HasherRT())).Persist(databaseData.Tree, hashOfHeader).AsBytes();
            await configuredStream.WriteAsync(data, 0, data.Length);

            configuredStream.Dispose();
            var compressed = outStream.ToArray();
            var aesKey = await databaseData.MasterKey.GenerateHashedKeyAsync(kdb4File.pbMasterSeed, kdb4File.pbTransformSeed, databaseData.KeyEncryptionRounds);
            var encrypted = EncryptDatabase(compressed.AsBuffer(), aesKey).AsBytes();
            datawriter.WriteBytes(encrypted);

            var written = datawriter.DetachBuffer();

            await FileIO.WriteBufferAsync(databaseFile, written);

            var cryptoStream = new CryptoRandomStream(CrsAlgorithm.Salsa20, kdb4File.pbProtectedStreamKey.AsBytes(),new SHA256HasherRT());
            var parser = new Kdb4Parser(cryptoStream);
            databaseData.Tree = parser.ParseAndDecode(databaseData.Tree.Document);
        }

        public Stream ConfigureStream(Stream stream)
        {
            //Stream inputStream = stream;
            Stream inputStream = new HashedBlockStream(stream,true);

            if (kdb4File.pwDatabase.Compression == PwCompressionAlgorithm.GZip)
            {
                inputStream = new GZipStream(inputStream, CompressionMode.Compress);
            }
            return inputStream;
        }

        public IBuffer EncryptDatabase(IBuffer source, IBuffer aesKey)
        {
            var symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var aesCryptoKey = symKeyProvider.CreateSymmetricKey(aesKey);
            return CryptographicEngine.Encrypt(aesCryptoKey, source, kdb4File.pbEncryptionIV);
        }
    }
}