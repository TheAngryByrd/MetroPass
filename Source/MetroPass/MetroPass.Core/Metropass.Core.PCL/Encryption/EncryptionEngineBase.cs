using System;
using System.Threading.Tasks;

namespace Metropass.Core.PCL.Encryption
{
    public interface IEncryptionEngine
    {    
        CryptoAlgoritmType AlgorithmType { get; set; }

        Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
        Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
    }
    public enum CryptoAlgoritmType
    {
        AES_ECB,
        AES_CBC_PKCS7
    }

    public interface IKeyTransformer
    {
        Task<byte[]> Transform(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
    }
}
