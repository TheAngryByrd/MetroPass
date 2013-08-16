using System;
using System.IO;
using System.Threading.Tasks;
using Metropass.Core.PCL.Cipher;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using Metropass.Core.PCL.Compression;
using PCLStorage;

namespace Metropass.Core.PCL.Model.Kdb4.Writer
{
    public class Kdb4Writer : IKdbWriter
    {
        public Kdb4File kdb4File;
        private Kdb4HeaderWriter _headerWriter;
        private readonly IEncryptionEngine _databaseEncryptor;

        private readonly IEncryptionEngine _keyEncryptor;

        private readonly ICanSHA256Hash _hasher;

        private readonly IGZipStreamFactory _gZipFactory;

        public Kdb4Writer(Kdb4HeaderWriter headerWriter,
            IEncryptionEngine databaseEncryptor,
            IEncryptionEngine keyEncryptor,
            ICanSHA256Hash hasher,
            IGZipStreamFactory gZipFactory)
        {
            this._gZipFactory = gZipFactory;
            this._hasher = hasher;
            this._keyEncryptor = keyEncryptor;
            this._databaseEncryptor = databaseEncryptor;
            _headerWriter = headerWriter;
        }

        static Random random = new Random();
        private byte[] GenerateBytes(int length)
        {
            var output = new byte[length];
            random.NextBytes(output);
            return output;
        }

        public async Task Write(PwDatabase databaseData, IFile databaseFile)
        {
             kdb4File = new Kdb4File(databaseData);

            ResetHeaderBytes();

            MemoryStream memStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memStream);
         
            _headerWriter.WriteHeaders(writer, kdb4File);

           var header = memStream.ToArray();
           var hashOfHeader = _hasher.Hash(header);


            var outStream = new MemoryStream();
            await outStream.WriteAsync(kdb4File.pbStreamStartBytes, 0, (int)kdb4File.pbStreamStartBytes.Length);
            var configuredStream = ConfigureStream(outStream);

            var persister = new Kdb4Persister(new CryptoRandomStream(CrsAlgorithm.Salsa20, kdb4File.pbProtectedStreamKey, _hasher));
            var data = persister.Persist(databaseData.Tree, hashOfHeader);
            await configuredStream.WriteAsync(data, 0, data.Length);

            configuredStream.Dispose();
            var compressed = outStream.ToArray();

            var keyGenerator = new KeyGenerator(
                _hasher, 
                 _keyEncryptor,
                databaseData.MasterKey, 
                databaseData.MasterKey.PercentComplete);
            var aesKey = await keyGenerator.GenerateHashedKeyAsync(kdb4File.pbMasterSeed, kdb4File.pbTransformSeed, (int)databaseData.KeyEncryptionRounds);

            var encrypted = await EncryptDatabase(compressed, aesKey);
            writer.Write(encrypted);

            var streamToWriteToFile = writer.BaseStream;
            streamToWriteToFile.Position = 0;
            var bytesToWrite = streamToWriteToFile.ToArray();

            var databaseStream = await databaseFile.OpenAsync(FileAccess.ReadAndWrite);
            databaseStream.SetLength(0);
            await databaseStream.FlushAsync();
            await databaseStream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
            await databaseStream.FlushAsync();
            databaseStream.Dispose();

            var cryptoStream = new CryptoRandomStream(CrsAlgorithm.Salsa20, kdb4File.pbProtectedStreamKey,_hasher);
            var parser = new Kdb4Parser(cryptoStream);
            databaseData.Tree = parser.ParseAndDecode(databaseData.Tree.Document);
        }
  
        private void ResetHeaderBytes()
        {
            kdb4File.pbMasterSeed = GenerateBytes(32);
            kdb4File.pbTransformSeed = GenerateBytes(32);
            kdb4File.pbEncryptionIV = GenerateBytes(16);
            kdb4File.pbProtectedStreamKey = GenerateBytes(32);
            kdb4File.pbStreamStartBytes = GenerateBytes(32);
        }

        public Stream ConfigureStream(Stream stream)
        {
            //Stream inputStream = stream;
            Stream inputStream = new HashedBlockStream(stream,true,_hasher);

            if (kdb4File.pwDatabase.Compression == PwCompressionAlgorithm.GZip)
            {
                inputStream = _gZipFactory.Compress(inputStream);
            }
            return inputStream;
        }

        public async Task<byte[]> EncryptDatabase(byte[] source, byte[] aesKey)
        {
            return await _databaseEncryptor.Encrypt(source, aesKey, kdb4File.pbEncryptionIV, 1, new NullableProgress<double>());
            //var symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
        }
    }
}