using System;
using System.Threading.Tasks;

namespace MetroPass.Core.Interfaces
{
    public interface IEncryptionEngine
    {
        CryptoAlgoritmType AlgorithmType { get; set; }

        Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
        Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
    }

    public abstract class EncryptionEngineBase : IEncryptionEngine
    {
        public CryptoAlgoritmType AlgorithmType { get; set; }

        public EncryptionEngineBase(CryptoAlgoritmType algorithmType)
        {
            AlgorithmType = algorithmType;
        }

        public abstract Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
        public abstract Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
    }

    public enum CryptoAlgoritmType
    {
        AES_ECB,
        AES_CBC_PKCS7
    }
}
