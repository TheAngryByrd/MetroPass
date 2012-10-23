using Framework;
using System;
using System.Linq;
using System.Text;
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

        public static string ByteArrayToHexString(byte[] byteArray)
        {
            if (byteArray == null) return null;

            int nLen = byteArray.Length;
            if (nLen == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            byte bt, btHigh, btLow;
            for (int i = 0; i < nLen; ++i)
            {
                bt = byteArray[i];
                btHigh = bt; btHigh >>= 4;
                btLow = (byte)(bt & 0x0F);

                if (btHigh >= 10) sb.Append((char)('A' + btHigh - 10));
                else sb.Append((char)('0' + btHigh));

                if (btLow >= 10) sb.Append((char)('A' + btLow - 10));
                else sb.Append((char)('0' + btLow));
            }

            return sb.ToString();
        }
    }
}