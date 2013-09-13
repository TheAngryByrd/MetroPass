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
        public WinRTCrypto(CryptoAlgoritmType algorithmType)            
        {
            AlgorithmType = algorithmType;
        }

        public CryptoAlgoritmType AlgorithmType
        {
            get;
            set;
        }

        public async Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
            return await Process(data, key, iv, rounds, percentComplete, CryptographicEngine.Encrypt);
        }

        public async Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
            return await Process(data, key, iv, rounds, percentComplete, CryptographicEngine.Decrypt);
        }

        private Task<byte[]> Process(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete, Func<CryptographicKey, IBuffer, IBuffer, IBuffer> cryptoAction)
        {
            return Task.Run(() =>
            {
                SymmetricKeyAlgorithmProvider symKeyProvider = GetSymmetricKeyAlgorithmProvider();
                var transformSeedKey = symKeyProvider.CreateSymmetricKey(key.AsBuffer());

                for (var i = 0; i < rounds; ++i)
                {
                    if (i % 5000 == 0)
                    {
                        percentComplete.Report(i / rounds * 100);

                    }
                    data = cryptoAction(transformSeedKey, data.AsBuffer(), iv.AsBuffer()).AsBytes();

                }
                percentComplete.Report(100);

                return data;

            });

        }

        private SymmetricKeyAlgorithmProvider GetSymmetricKeyAlgorithmProvider()
        {
            SymmetricKeyAlgorithmProvider symmetricKeyAlgorithm = null;

            switch (AlgorithmType)
            {
                case CryptoAlgoritmType.AES_ECB:
                    symmetricKeyAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
                    break;
                case CryptoAlgoritmType.AES_CBC_PKCS7:
                    symmetricKeyAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                    break;
            }
            return symmetricKeyAlgorithm;
        }


    }
}
