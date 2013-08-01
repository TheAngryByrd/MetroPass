using System.IO;

namespace Metropass.Core.PCL.Compression
{
    public interface IGZipStreamFactory
    {
        Stream Decompress(Stream binaryStream);
    }
}
