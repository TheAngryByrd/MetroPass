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

            m_format = kdbFormat;

            var hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hashAlgorithmProvider.CreateHash();

           
        }

        public static ushort BytesToUInt16(byte[] pb)
        {
            Debug.Assert((pb != null) && (pb.Length == 2));
            if (pb == null) throw new ArgumentNullException("pb");
            if (pb.Length != 2) throw new ArgumentException();

            return (ushort)((ushort)pb[0] | ((ushort)pb[1] << 8));
        }

        public bool ReadHeaderField(IDataReader reader)
        {
            byte btFieldID = reader.ReadByte();
            var btSize = new byte[2];
            reader.ReadBytes(btSize);
           // btSize = new byte[2] { btSize[1], btSize[0] };
            var uSize = BytesToUInt16(btSize);
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
                    //SetCipher(pbData);
                    break;

                case Kdb4HeaderFieldID.CompressionFlags:
                    //SetCompressionFlags(pbData);
                    break;

                case Kdb4HeaderFieldID.MasterSeed:
                    //m_pbMasterSeed = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformSeed:
                    //m_pbTransformSeed = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformRounds:
                    //m_pwDatabase.KeyEncryptionRounds = MemUtil.BytesToUInt64(pbData);
                    break;

                case Kdb4HeaderFieldID.EncryptionIV:
                    //m_pbEncryptionIV = pbData;
                    break;

                case Kdb4HeaderFieldID.ProtectedStreamKey:
                    //m_pbProtectedStreamKey = pbData;
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.StreamStartBytes:
                    //m_pbStreamStartBytes = pbData;
                    break;

                case Kdb4HeaderFieldID.InnerRandomStreamID:
                    //SetInnerRandomStreamID(pbData);
                    break;

                default:
                    break;
            }
            return bResult;
        }
    }
}
