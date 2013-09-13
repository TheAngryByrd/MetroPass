using System;
using System.Threading.Tasks;

namespace Metropass.Core.PCL.Encryption
{
    public interface IKeyTransformer
    {
        Task<byte[]> Transform(byte[] data, byte[] key, double rounds, IProgress<double> percentComplete);
    }
}