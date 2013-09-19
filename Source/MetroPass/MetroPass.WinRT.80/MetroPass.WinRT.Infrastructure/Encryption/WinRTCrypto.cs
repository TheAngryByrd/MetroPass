using System;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Metropass.Core.PCL.Encryption;

namespace MetroPass.WinRT.Infrastructure.Encryption
{
    public class WinRTCrypto : IEncryptionEngine
    {
        public CryptoAlgoritmType AlgorithmType
        {
            get;
            set;
        }

        public async Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            return await Process(data, key, iv, CryptographicEngine.Encrypt);
        }

        public async Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            return await Process(data, key, iv, CryptographicEngine.Decrypt);
        }

        private Task<byte[]> Process(byte[] data, byte[] key, byte[] iv, Func<CryptographicKey, IBuffer, IBuffer, IBuffer> cryptoAction)
        {
            return Task.Run(() =>
            {
                SymmetricKeyAlgorithmProvider symKeyProvider = GetSymmetricKeyAlgorithmProvider();
                var transformSeedKey = symKeyProvider.CreateSymmetricKey(key.AsBuffer());
     
                data = cryptoAction(transformSeedKey, data.AsBuffer(), iv.AsBuffer()).AsBytes();

                return data;
            });

        }

        private SymmetricKeyAlgorithmProvider GetSymmetricKeyAlgorithmProvider()
        {
            SymmetricKeyAlgorithmProvider symmetricKeyAlgorithm = null;
            symmetricKeyAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);      
            return symmetricKeyAlgorithm;
        }


    }
}
