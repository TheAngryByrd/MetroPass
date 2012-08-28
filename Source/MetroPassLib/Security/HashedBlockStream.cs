using MetroPassLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPassLib.Security
{
    public sealed class HashedBlockStream : Stream
    {
        private const int m_nDefaultBufferSize = 1024 * 1024; // 1 MB

        private Stream m_sBaseStream;
        private bool m_bWriting;
        private bool m_bVerify;
        private bool m_bEos = false;

        private BinaryReader m_brInput;
        private BinaryWriter m_bwOutput;

        private byte[] m_pbBuffer;
        private int m_nBufferPos = 0;

        private uint m_uBufferIndex = 0;

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
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public HashedBlockStream(Stream sBaseStream, bool bWriting)
        {
            Initialize(sBaseStream, bWriting, 0, true);
        }

        public HashedBlockStream(Stream sBaseStream, bool bWriting, int nBufferSize)
        {
            Initialize(sBaseStream, bWriting, nBufferSize, true);
        }

        public HashedBlockStream(Stream sBaseStream, bool bWriting, int nBufferSize,
            bool bVerify)
        {
            Initialize(sBaseStream, bWriting, nBufferSize, bVerify);
        }

        private void Initialize(Stream sBaseStream, bool bWriting, int nBufferSize,
            bool bVerify)
        {
            if (sBaseStream == null) throw new ArgumentNullException("sBaseStream");
            if (nBufferSize < 0) throw new ArgumentOutOfRangeException("nBufferSize");

            if (nBufferSize == 0) nBufferSize = m_nDefaultBufferSize;

            m_sBaseStream = sBaseStream;
            m_bWriting = bWriting;
            m_bVerify = bVerify;

            UTF8Encoding utf8 = new UTF8Encoding(false, false);
            if (m_bWriting == false) // Reading mode
            {
                if (m_sBaseStream.CanRead == false)
                    throw new InvalidOperationException();

                m_brInput = new BinaryReader(sBaseStream, utf8);

                m_pbBuffer = new byte[0];
            }
            else // Writing mode
            {
                if (m_sBaseStream.CanWrite == false)
                    throw new InvalidOperationException();

                m_bwOutput = new BinaryWriter(sBaseStream, utf8);

                m_pbBuffer = new byte[nBufferSize];
            }
        }

        public override void Flush()
        {
            if (m_bWriting) m_bwOutput.Flush();
        }

        public  void Close()
        {
            if (m_sBaseStream != null)
            {
                if (m_bWriting == false) // Reading mode
                {

                    m_brInput = null;
                }
                else // Writing mode
                {
                    if (m_nBufferPos == 0) // No data left in buffer
                        WriteHashedBlock(); // Write terminating block
                    else
                    {
                        WriteHashedBlock(); // Write remaining buffered data
                        WriteHashedBlock(); // Write terminating block
                    }

                    Flush();

                    m_bwOutput = null;
                }


                m_sBaseStream = null;
            }
        }

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

            int nRemaining = nCount;
            while (nRemaining > 0)
            {
                if (m_nBufferPos == m_pbBuffer.Length)
                {
                    if (ReadHashedBlock() == false)
                        return nCount - nRemaining; // Bytes actually read
                }

                int nCopy = Math.Min(m_pbBuffer.Length - m_nBufferPos, nRemaining);

                Array.Copy(m_pbBuffer, m_nBufferPos, pbBuffer, nOffset, nCopy);

                nOffset += nCopy;
                m_nBufferPos += nCopy;

                nRemaining -= nCopy;
            }

            return nCount;
        }

        private bool ReadHashedBlock()
        {
            if (m_bEos) return false; // End of stream reached already

            m_nBufferPos = 0;

            if (m_brInput.ReadUInt32() != m_uBufferIndex)
                throw new InvalidDataException();
            ++m_uBufferIndex;

            byte[] pbStoredHash = m_brInput.ReadBytes(32);
            if ((pbStoredHash == null) || (pbStoredHash.Length != 32))
                throw new InvalidDataException();

            int nBufferSize = 0;
            try { nBufferSize = m_brInput.ReadInt32(); }
            catch (NullReferenceException) // Mono bug workaround (LaunchPad 783268)
            {
               
            }

            if (nBufferSize < 0)
                throw new InvalidDataException();

            if (nBufferSize == 0)
            {
                for (int iHash = 0; iHash < 32; ++iHash)
                {
                    if (pbStoredHash[iHash] != 0)
                        throw new InvalidDataException();
                }

                m_bEos = true;
                m_pbBuffer = new byte[0];
                return false;
            }

            m_pbBuffer = m_brInput.ReadBytes(nBufferSize);
            if ((m_pbBuffer == null) || ((m_pbBuffer.Length != nBufferSize) && m_bVerify))
                throw new InvalidDataException();

            if (m_bVerify)
            {

                byte[] pbComputedHash = SHA256Hasher.Hash(m_pbBuffer.AsBuffer()).AsBytes();
                if ((pbComputedHash == null) || (pbComputedHash.Length != 32))
                    throw new InvalidOperationException();

                for (int iHashPos = 0; iHashPos < 32; ++iHashPos)
                {
                    if (pbStoredHash[iHashPos] != pbComputedHash[iHashPos])
                        throw new InvalidDataException();
                }
            }

            return true;
        }

        public override void Write(byte[] pbBuffer, int nOffset, int nCount)
        {
            if (!m_bWriting) throw new InvalidOperationException();

            while (nCount > 0)
            {
                if (m_nBufferPos == m_pbBuffer.Length)
                    WriteHashedBlock();

                int nCopy = Math.Min(m_pbBuffer.Length - m_nBufferPos, nCount);

                Array.Copy(pbBuffer, nOffset, m_pbBuffer, m_nBufferPos, nCopy);

                nOffset += nCopy;
                m_nBufferPos += nCopy;

                nCount -= nCopy;
            }
        }

        private void WriteHashedBlock()
        {
            m_bwOutput.Write(m_uBufferIndex);
            ++m_uBufferIndex;

            if (m_nBufferPos > 0)
            {



				byte[] pbHash;
                if (m_nBufferPos == m_pbBuffer.Length)

                    pbHash = SHA256Hasher.Hash(m_pbBuffer.AsBuffer()).AsBytes();
                else
                {
                    byte[] pbData = new byte[m_nBufferPos];
                    Array.Copy(m_pbBuffer, 0, pbData, 0, m_nBufferPos);
                    pbHash = SHA256Hasher.Hash(pbData.AsBuffer()).AsBytes();
                }


                m_bwOutput.Write(pbHash);
            }
            else
            {
                m_bwOutput.Write((ulong)0); // Zero hash
                m_bwOutput.Write((ulong)0);
                m_bwOutput.Write((ulong)0);
                m_bwOutput.Write((ulong)0);
            }

            m_bwOutput.Write(m_nBufferPos);

            if (m_nBufferPos > 0)
                m_bwOutput.Write(m_pbBuffer, 0, m_nBufferPos);

            m_nBufferPos = 0;
        }
    }
}
