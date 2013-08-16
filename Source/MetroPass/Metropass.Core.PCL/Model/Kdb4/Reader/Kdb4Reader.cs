using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Metropass.Core.PCL.Cipher;
using Metropass.Core.PCL.Compression;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Keys;

namespace Metropass.Core.PCL.Model.Kdb4.Reader
{
    public class Kdb4Reader : IKdbReader
    {
        private readonly ICanSHA256Hash _hasher;

        private readonly IEncryptionEngine _encryptionEngine;

        private readonly IEncryptionEngine _keyDecryptor;

        private readonly IGZipStreamFactory _gZipFactory;

        public Kdb4File file { get; set; }

        public Kdb4Reader(Kdb4File kdb4File, 
            IEncryptionEngine databaseDecryptor, 
            IEncryptionEngine keyDecryptor, 
            ICanSHA256Hash hasher,
            IGZipStreamFactory gZipFactory)
        {            
            file = kdb4File;
            _encryptionEngine = databaseDecryptor;
            _keyDecryptor = keyDecryptor;
            _hasher = hasher;
            _gZipFactory = gZipFactory;
        }

        public async Task<IKdbTree> Load(Stream source)
        {
            if (source == null) throw new ArgumentNullException("sSource");

            ReadHeader(source);
            var aesKey = await GenerateAESKey();
            var decryptedDatabase = await DecryptDatabase(source.ToArray(), aesKey);
            var decompressEdDatabase = ConfigureStream(decryptedDatabase);
            var crypto = GenerateCryptoRandomStream();
            var parser = new Kdb4Parser(crypto);

            return parser.ParseStream(decompressEdDatabase);
        }

        public CryptoRandomStream GenerateCryptoRandomStream()
        {
            if (file.pbProtectedStreamKey == null)
            {
                throw new SecurityException("Invalid protected stream key!");
            }
            return new CryptoRandomStream(file.craInnerRandomStream, file.pbProtectedStreamKey, _hasher);
        }

        public Stream ConfigureStream(Stream decryptedDatabase)
        {

            Stream inputStream = new HashedBlockStream(decryptedDatabase, false, 0, false, _hasher);

            if (file.pwDatabase.Compression == PwCompressionAlgorithm.GZip)
            {
                inputStream = _gZipFactory.Decompress(inputStream);
            }
            return inputStream;

        }

        public async Task<Stream> DecryptDatabase(byte[] source, byte[] aesKey)
        {
            var unreadData = source;
            byte[] decryptedDatabase = null;
            try
            {
                _encryptionEngine.AlgorithmType = CryptoAlgoritmType.AES_CBC_PKCS7;
                decryptedDatabase = await _encryptionEngine.Decrypt(source, aesKey, file.pbEncryptionIV, 1, new NullableProgress<double>());

            }
            catch (Exception e)
            {
                throw new SecurityException("There was a problem opening your database", e);
            }
            var startBytes = new byte[32];
            var databaseReader = new MemoryStream(decryptedDatabase);

            databaseReader.ReadBytes(startBytes);
            var headerStartBytes = file.pbStreamStartBytes;
            for (int iStart = 0; iStart < 32; ++iStart)
            {
                if (startBytes[iStart] != headerStartBytes[iStart])
                    throw new Exception();
            }
            return databaseReader;
        }

        public async Task<byte[]> GenerateAESKey()
        {
            var pwDatabase = file.pwDatabase;
            var keyGenerator = new KeyGenerator(
               _hasher,
               _keyDecryptor,
               pwDatabase.MasterKey,
               pwDatabase.MasterKey.PercentComplete);
            var aesKey = await keyGenerator.GenerateHashedKeyAsync(file.pbMasterSeed, file.pbTransformSeed, (int)pwDatabase.KeyEncryptionRounds);

            return aesKey;
        }
        public void ReadHeader(Stream reader)
        {
            while (true)
            {
                if (this.ReadHeaderField(reader) == false) { break; }
            }

        }

        public bool ReadHeaderField(Stream reader)
        {
            byte btFieldID = Convert.ToByte(reader.ReadByte());
            var btSize = new byte[2];
            reader.ReadBytes(btSize);

            var uSize = BitConverter.ToUInt16(btSize, 0);
            Kdb4HeaderFieldID kdbID = (Kdb4HeaderFieldID)btFieldID;
            byte[] pbData = null;
            if (uSize > 0)
            {
                pbData = new byte[uSize];
                reader.ReadBytes(pbData);
            }

            bool bResult = true;
            switch (kdbID)
            {
                case Kdb4HeaderFieldID.EndOfHeader:
                    bResult = false; // Returning false indicates end of header
                    break;

                case Kdb4HeaderFieldID.CipherID:
                    SetCipher(pbData);
                    break;

                case Kdb4HeaderFieldID.CompressionFlags:
                    SetCompressionFlags(pbData);
                    break;

                case Kdb4HeaderFieldID.MasterSeed:
                    file.pbMasterSeed = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformSeed:
                    file.pbTransformSeed = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformRounds:
                    file.pwDatabase.KeyEncryptionRounds = BitConverter.ToUInt64(pbData, 0);
                    break;

                case Kdb4HeaderFieldID.EncryptionIV:
                    file.pbEncryptionIV = pbData;
                    break;

                case Kdb4HeaderFieldID.ProtectedStreamKey:
                    file.pbProtectedStreamKey = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.StreamStartBytes:
                    file.pbStreamStartBytes = pbData;
                    break;

                case Kdb4HeaderFieldID.InnerRandomStreamID:
                    SetInnerRandomStreamID(pbData);
                    break;

                default:
                    break;
            }
            return bResult;
        }

        private void SetInnerRandomStreamID(byte[] pbID)
        {
            uint uID = BitConverter.ToUInt32(pbID, 0);
            if (uID >= (uint)CrsAlgorithm.Count)
                throw new FormatException();

            file.craInnerRandomStream = (CrsAlgorithm)uID;
        }

        private void SetCompressionFlags(byte[] pbFlags)
        {
            int nID = (int)BitConverter.ToUInt32(pbFlags, 0);
            if ((nID < 0) || (nID >= (int)PwCompressionAlgorithm.Count))
                throw new FormatException();

            file.pwDatabase.Compression = (PwCompressionAlgorithm)nID;
        }

        private void SetCipher(byte[] pbID)
        {
            if ((pbID == null) || (pbID.Length != 16))
                throw new FormatException();

            file.pwDatabase.DataCipherUuid = new PwUuid(new byte[]{
                        0x31, 0xC1, 0xF2, 0xE6, 0xBF, 0x71, 0x43, 0x50,
                        0xBE, 0x58, 0x05, 0x21, 0x6A, 0xFC, 0x5A, 0xFF });//new PwUuid(pbID);
        }


    }
}
