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

           await _headerWriter.WriteHeaders(datawriter, kdb4File);

           var aesKey = await databaseData.MasterKey.GenerateHashedKeyAsync(kdb4File.pbMasterSeed, kdb4File.pbTransformSeed, databaseData.KeyEncryptionRounds);
           await outa.WriteAsync(kdb4File.pbStreamStartBytes);
  
  
            var data = new Kdb4Persister().Persist(databaseData.Tree);
            var encrypted = EncryptDatabase(data, aesKey);
         
            //await memoryStream2.WriteAsync(EncryptDatabase(data,aesKey));
            var configuredStream = ConfigureStream(outa.AsStreamForWrite());
           await configuredStream.WriteAsync(encrypted.AsBytes(), 0, (int)encrypted.Length);



          //  XmlWriter.Create(configuredStream);

          //  await configuredStream.WriteAsync(data.AsBytes(), 0, (int)data.Length);
          //  //await configuredStream.FlushAsync();
          //  var x = new DataWriter( configuredStream.AsOutputStream());
          //  //await x.FlushAsync();
           
          //  var buff = x.DetachBuffer();
          // // var bufferedData = new DataReader(configuredStream.AsInputStream()).DetachBuffer();
          ////  var encryptedDatabase = EncryptDatabase(, aesKey);
           await fs.CommitAsync();
        }

        public Stream ConfigureStream(Stream stream)
        {


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
