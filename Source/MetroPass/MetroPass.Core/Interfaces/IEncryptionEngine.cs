using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MetroPass.Core.Interfaces
{
    internal interface IEncryptionEngine
    {
        CryptoAlgoritmType AlgorithmType { get; set; }

        Task<IBuffer> Encrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete);
        Task<IBuffer> Decrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete);
    }

    public abstract class EncryptionEngine : IEncryptionEngine
    {
        public CryptoAlgoritmType AlgorithmType { get; set; }

        public EncryptionEngine(CryptoAlgoritmType algorithmType)
        {
            AlgorithmType = algorithmType;
        }

        public abstract Task<IBuffer> Encrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete);
        public abstract Task<IBuffer> Decrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete);
    }

    public enum CryptoAlgoritmType
    {
        AES_ECB,
        AES_CBC_PKCS7
    }
}
