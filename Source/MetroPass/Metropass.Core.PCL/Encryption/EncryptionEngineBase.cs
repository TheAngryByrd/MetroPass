using System;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;

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
}
