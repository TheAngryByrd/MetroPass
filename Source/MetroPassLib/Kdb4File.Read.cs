using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using MetroPassLib.Helpers;

namespace MetroPassLib
{

    public partial class Kdb4File
    {
        public async Task Load(IDataReader source, Kdb4Format kdbFormat)
        {
            Debug.Assert(source != null);
            if (source == null) throw new ArgumentNullException("sSource");

            kdb4Format = kdbFormat;
            
            ReadHeader(source);
            var aesKey = await GenerateAESKey();
            var decryoptedDatabaseBuffer = DecryptDatabase(source.DetachBuffer(), aesKey);

        
            
        }

        public IDataReader DecryptDatabase(IBuffer source, IBuffer aesKey)
        {
            var symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var aesCryptoKey = symKeyProvider.CreateSymmetricKey(aesKey);
            var unreadData = source;
            var decryptedDatabase = CryptographicEngine.Decrypt(aesCryptoKey, unreadData, pbEncryptionIV);
            var databaseReader = DataReader.FromBuffer(decryptedDatabase);

            var startBytes = databaseReader.ReadBuffer(32).AsBytes();
            var headerStartBytes = this.pbStreamStartBytes.AsBytes();
            for (int iStart = 0; iStart < 32; ++iStart)
            {
                if (startBytes[iStart] != headerStartBytes[iStart])
                    throw new Exception();
            }
            return databaseReader;
        }

        public async Task<IBuffer> GenerateAESKey()
        {
            var hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hashAlgorithmProvider.CreateHash();

            hash.Append(pbMasterSeed);
            var generatedKey = await pwDatabase.MasterKey.GenerateKeyAsync(pbTransformSeed, pwDatabase.KeyEncryptionRounds);
            var masterKey = generatedKey;
            hash.Append(masterKey);

            var aesKey = hash.GetValueAndReset();
            return aesKey;
        }
        public void ReadHeader(IDataReader reader)
        {
            reader.ReadBytes(new byte[12]);


            while (true)
            {
                if (this.ReadHeaderField(reader) == false) { break; }
            }    
            
        }

        public bool ReadHeaderField(IDataReader reader)
        {
            byte btFieldID = reader.ReadByte();
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
                    pbMasterSeed = pbData.AsBuffer();
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformSeed:
                    pbTransformSeed = pbData.AsBuffer();
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformRounds:
                    pwDatabase.KeyEncryptionRounds = BitConverter.ToUInt64(pbData, 0);
                    break;

                case Kdb4HeaderFieldID.EncryptionIV:
                    pbEncryptionIV = pbData.AsBuffer();
                    break;

                case Kdb4HeaderFieldID.ProtectedStreamKey:
                    pbProtectedStreamKey = pbData.AsBuffer();
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.StreamStartBytes:
                    pbStreamStartBytes = pbData.AsBuffer();
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

            craInnerRandomStream = (CrsAlgorithm)uID;
        }

        private void SetCompressionFlags(byte[] pbFlags)
        {
            int nID = (int)BitConverter.ToUInt32(pbFlags, 0);
            if ((nID < 0) || (nID >= (int)PwCompressionAlgorithm.Count))
                throw new FormatException();

            pwDatabase.Compression = (PwCompressionAlgorithm)nID;
        }

        private void SetCipher(byte[] pbID)
        {
            if ((pbID == null) || (pbID.Length != 16))
                throw new FormatException();

            pwDatabase.DataCipherUuid = new PwUuid(pbID);
        }



    }
}
