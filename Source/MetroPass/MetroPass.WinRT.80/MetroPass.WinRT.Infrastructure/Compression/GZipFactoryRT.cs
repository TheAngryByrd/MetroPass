using System;
using System.IO;
using System.IO.Compression;
using Metropass.Core.PCL.Compression;

namespace MetroPass.WinRT.Infrastructure.Compression
{
    public class GZipFactoryRT : IGZipStreamFactory
    {
        public Stream Compress(Stream binaryStream)
        {
            return new GZipStream(binaryStream, CompressionMode.Compress);
        }

        public Stream Decompress(Stream binaryStream)
        {
            return new GZipStream(binaryStream, CompressionMode.Decompress);
        }
    }
}
