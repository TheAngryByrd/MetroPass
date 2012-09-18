using Framework;
using MetroPass.Core.Helpers.Cipher;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Compression;
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
        public async Task Write(PwDatabase databaseData, Windows.Storage.IStorageFile database)
        {
             kdb4File = new Kdb4File(databaseData);
            kdb4File.pbMasterSeed = CryptographicBuffer.GenerateRandom(32);
            kdb4File.pbTransformSeed = CryptographicBuffer.GenerateRandom(32);
            kdb4File.pbEncryptionIV = CryptographicBuffer.GenerateRandom(16);
            kdb4File.pbProtectedStreamKey = CryptographicBuffer.GenerateRandom(32);
            kdb4File.pbStreamStartBytes = CryptographicBuffer.GenerateRandom(32);
            
            var fs = await database.OpenTransactedWriteAsync();
            var outa = fs.Stream;
            var datawriter = new DataWriter(outa);
            datawriter.ByteOrder = ByteOrder.LittleEndian;

           await _headerWriter.WriteHeaders(datawriter, kdb4File);
           //datawriter.WriteBuffer(kdb4File.pbStreamStartBytes);
           await datawriter.StoreAsync();

            var outStream = new MemoryStream();
            await outStream.WriteAsync(kdb4File.pbStreamStartBytes.AsBytes(), 0, (int)kdb4File.pbStreamStartBytes.Length);
            var configuredStream = ConfigureStream(outStream);

            var data = new Kdb4Persister(new CryptoRandomStream(CrsAlgorithm.Salsa20, kdb4File.pbProtectedStreamKey.AsBytes())).Persist(databaseData.Tree).AsBytes();
            await configuredStream.WriteAsync(data, 0, data.Length);

            configuredStream.Dispose();
            var compressed = outStream.ToArray();
            var aesKey = await databaseData.MasterKey.GenerateHashedKeyAsync(kdb4File.pbMasterSeed, kdb4File.pbTransformSeed, databaseData.KeyEncryptionRounds);
            var encrypted = EncryptDatabase(compressed.AsBuffer(), aesKey).AsBytes();
            datawriter.WriteBytes(encrypted);
            await datawriter.StoreAsync();
            await datawriter.FlushAsync();

           await fs.CommitAsync();
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
