using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPassLib
{

    public partial class Kdb4File
    {
        public void Load(Stream sSource, Kdb4Format kdbFormat)
        {
            Debug.Assert(sSource != null);
            if (sSource == null) throw new ArgumentNullException("sSource");

            kdb4Format = kdbFormat;

            var hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hashAlgorithmProvider.CreateHash();

           
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
                    pbMasterSeed = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformSeed:
                    pbTransformSeed = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformRounds:
                    pwDatabase.KeyEncryptionRounds = BitConverter.ToUInt64(pbData, 0);
                    break;

                case Kdb4HeaderFieldID.EncryptionIV:
                    pbEncryptionIV = pbData;
                    break;

                case Kdb4HeaderFieldID.ProtectedStreamKey:
                    pbProtectedStreamKey = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.StreamStartBytes:
                    pbStreamStartBytes = pbData;
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



        public byte[] pbEncryptionIV { get; set; }

        public byte[] pbProtectedStreamKey { get; set; }

        public byte[] pbStreamStartBytes { get; set; }

        public CrsAlgorithm craInnerRandomStream { get; set; }

        public byte[] pbTransformSeed { get; set; }
    }
}
