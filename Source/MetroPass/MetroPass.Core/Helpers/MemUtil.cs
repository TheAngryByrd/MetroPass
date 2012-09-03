using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MetroPass.Core.Helpers
{
    public class MemUtil
    {
        public static void XorArray(byte[] pbSource, int nSourceOffset,
byte[] pbBuffer, int nBufferOffset, int nLength)
        {
            if (pbSource == null) throw new ArgumentNullException("pbSource");
            if (nSourceOffset < 0) throw new ArgumentException();
            if (pbBuffer == null) throw new ArgumentNullException("pbBuffer");
            if (nBufferOffset < 0) throw new ArgumentException();
            if (nLength < 0) throw new ArgumentException();
            if ((nSourceOffset + nLength) > pbSource.Length) throw new ArgumentException();
            if ((nBufferOffset + nLength) > pbBuffer.Length) throw new ArgumentException();

            for (int i = 0; i < nLength; ++i)
                pbBuffer[nBufferOffset + i] ^= pbSource[nSourceOffset + i];
        }

        public static IBuffer HexStringToByteArray(string strHex)
        {
            return Enumerable.Range(0, strHex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(strHex.Substring(x, 2), 16))
                    .ToArray().AsBuffer();
        }
    }
}
