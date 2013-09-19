using System;
using System.Threading.Tasks;

namespace Metropass.Core.PCL.Encryption
{
    public interface IEncryptionEngine
    {    
        CryptoAlgoritmType AlgorithmType { get; set; }

        Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv);
        Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv);
    }
    public enum CryptoAlgoritmType
    {
        AES_ECB,
        AES_CBC_PKCS7
    }
}
