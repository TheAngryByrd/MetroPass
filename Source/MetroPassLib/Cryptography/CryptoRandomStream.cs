using MetroPassLib.Cryptography.Cipher;
using MetroPassLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPassLib.Cryptography
{
    /// <summary>
    /// Algorithms supported by <c>CryptoRandomStream</c>.
    /// </summary>
    public enum CrsAlgorithm
    {
        /// <summary>
        /// Not supported.
        /// </summary>
        Null = 0,

        /// <summary>
        /// A variant of the ARCFour algorithm (RC4 incompatible).
        /// </summary>
        ArcFourVariant = 1,

        /// <summary>
        /// Salsa20 stream cipher algorithm.
        /// </summary>
        Salsa20 = 2,

        Count = 3
    }

    /// <summary>
    /// A random stream class. The class is initialized using random
    /// bytes provided by the caller. The produced stream has random
    /// properties, but for the same seed always the same stream
    /// is produced, i.e. this class can be used as stream cipher.
    /// </summary>
    public sealed class CryptoRandomStream
    {
        private CrsAlgorithm m_crsAlgorithm;

        private byte[] m_pbState = null;
        private byte m_i = 0;
        private byte m_j = 0;

        private Salsa20Cipher m_salsa20 = null;

        /// <summary>
        /// Construct a new cryptographically secure random stream object.
        /// </summary>
        /// <param name="genAlgorithm">Algorithm to use.</param>
        /// <param name="pbKey">Initialization key. Must not be <c>null</c> and
        /// must contain at least 1 byte.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the
        /// <paramref name="pbKey" /> parameter is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the
        /// <paramref name="pbKey" /> parameter contains no bytes or the
        /// algorithm is unknown.</exception>
        public CryptoRandomStream(CrsAlgorithm genAlgorithm, byte[] pbKey)
        {
            m_crsAlgorithm = genAlgorithm;

          

            uint uKeyLen = (uint)pbKey.Length;
          

            if (genAlgorithm == CrsAlgorithm.ArcFourVariant)
            {
                // Fill the state linearly
                m_pbState = new byte[256];
                for (uint w = 0; w < 256; ++w) m_pbState[w] = (byte)w;

                unchecked
                {
                    byte j = 0, t;
                    uint inxKey = 0;
                    for (uint w = 0; w < 256; ++w) // Key setup
                    {
                        j += (byte)(m_pbState[w] + pbKey[inxKey]);

                        t = m_pbState[0]; // Swap entries
                        m_pbState[0] = m_pbState[j];
                        m_pbState[j] = t;

                        ++inxKey;
                        if (inxKey >= uKeyLen) inxKey = 0;
                    }
                }

                GetRandomBytes(512); // Increases security, see cryptanalysis
            }
            else if (genAlgorithm == CrsAlgorithm.Salsa20)
            {


                byte[] pbKey32 = SHA256Hasher.Hash(pbKey.AsBuffer()).AsBytes();
                byte[] pbIV = new byte[]{ 0xE8, 0x30, 0x09, 0x4B,
					0x97, 0x20, 0x5D, 0x2A }; // Unique constant

                m_salsa20 = new Salsa20Cipher(pbKey32, pbIV);
            }
            else // Unknown algorithm
            {
     
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Get <paramref name="uRequestedCount" /> random bytes.
        /// </summary>
        /// <param name="uRequestedCount">Number of random bytes to retrieve.</param>
        /// <returns>Returns <paramref name="uRequestedCount" /> random bytes.</returns>
        public byte[] GetRandomBytes(uint uRequestedCount)
        {
            if (uRequestedCount == 0) return new byte[0];

            byte[] pbRet = new byte[uRequestedCount];

            if (m_crsAlgorithm == CrsAlgorithm.ArcFourVariant)
            {
                unchecked
                {
                    for (uint w = 0; w < uRequestedCount; ++w)
                    {
                        ++m_i;
                        m_j += m_pbState[m_i];

                        byte t = m_pbState[m_i]; // Swap entries
                        m_pbState[m_i] = m_pbState[m_j];
                        m_pbState[m_j] = t;

                        t = (byte)(m_pbState[m_i] + m_pbState[m_j]);
                        pbRet[w] = m_pbState[t];
                    }
                }
            }
            else if (m_crsAlgorithm == CrsAlgorithm.Salsa20)
                m_salsa20.Encrypt(pbRet, pbRet.Length, false);
            else { }

            return pbRet;
        }

        public ulong GetRandomUInt64()
        {
            byte[] pb = GetRandomBytes(8);

            unchecked
            {
                return ((ulong)pb[0]) | ((ulong)pb[1] << 8) |
                    ((ulong)pb[2] << 16) | ((ulong)pb[3] << 24) |
                    ((ulong)pb[4] << 32) | ((ulong)pb[5] << 40) |
                    ((ulong)pb[6] << 48) | ((ulong)pb[7] << 56);
            }
        }


    }
}
