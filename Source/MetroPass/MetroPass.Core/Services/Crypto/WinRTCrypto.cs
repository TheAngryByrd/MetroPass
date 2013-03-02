using MetroPass.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPass.Core.Services.Crypto
{
    public class WinRTCrypto : EncryptionEngine
    {
        public WinRTCrypto(CryptoAlgoritmType algorithmType) : base(algorithmType)
        {
        }

        public override async Task<IBuffer> Encrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete)
        {
            return await Process(data, key, iv, rounds, percentComplete, CryptographicEngine.Encrypt);
        }


        public async override Task<IBuffer> Decrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete)
        {
            return await Process(data, key, iv, rounds, percentComplete, CryptographicEngine.Decrypt);
        }

        private Task<IBuffer> Process(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete, Func<CryptographicKey, IBuffer, IBuffer, IBuffer> cryptoAction)
        {
            return Task.Run(() =>
            {
                SymmetricKeyAlgorithmProvider symKeyProvider = GetSymmetricKeyAlgorithmProvider();
                var transformSeedKey = symKeyProvider.CreateSymmetricKey(key);
     
                for (var i = 0; i < rounds; ++i)
                {
                    if (i % 5000 == 0)
                    {
                        percentComplete.Report(i / rounds * 100);

                    }
                    data = cryptoAction(transformSeedKey, data, iv);

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
                    break;
            }
            return symmetricKeyAlgorithm;
        }


    }
}
