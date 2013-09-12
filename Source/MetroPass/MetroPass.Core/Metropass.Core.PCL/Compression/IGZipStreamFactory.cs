using System.IO;

namespace Metropass.Core.PCL.Compression
{
    public interface IGZipStreamFactory
    {
        Stream Compress(Stream binaryStream);

        Stream Decompress(Stream binaryStream);
    }
}
