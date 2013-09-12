using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using Metropass.Core.PCL.Compression;

namespace MetroPass.WP8.Infrastructure.Compression
{
    public class GZipFactoryWP8 : IGZipStreamFactory
    {
        public Stream Compress(Stream binaryStream)
        {
            return new GZipOutputStream(binaryStream);            
        }

        public Stream Decompress(Stream binaryStream)
        {
            return new GZipInputStream(binaryStream);      
        }
    }
}
