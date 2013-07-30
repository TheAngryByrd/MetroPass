using System;
using System.Linq;
using MetroPass.Core.Interfaces;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Threading.Tasks;

namespace MetroPass.WinRT.Infrastructure.Encryption
{
    public class MultiThreadedBouncyCastleCrypto : EncryptionEngineBase
    {
        public MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType algorithmType)
            : base(algorithmType)
        {
        }

        public async override Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
            return await ProcessMultiThreaded(true, data, key, rounds, percentComplete);
        }

        public async override Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
            return await ProcessMultiThreaded(false, data, key, rounds, percentComplete);
        }

        private async Task<byte[]> ProcessMultiThreaded(bool encrypt, byte[] data, byte[] key, double rounds, IProgress<double> percentComplete)
        {
            var bData = data;
            var bKey = key;

            var t1 = Task.Run(() =>
            {
                IBufferedCipher cipher = GetCipher(encrypt, key);

                return Process(bData.Take(16).ToArray(), rounds, percentComplete, cipher);
            });
            var t2 = Task.Run(() =>
            {
                IBufferedCipher cipher = GetCipher(encrypt, key);

                return Process(bData.Skip(16).Take(16).ToArray(), rounds, percentComplete, cipher);
            });

            await Task.WhenAll(t1, t2);

            return t1.Result.Concat(t2.Result).ToArray();
        }

        private byte[] Process(byte[] data, double rounds, IProgress<double> percentComplete, IBufferedCipher cipher)
        {


            var byteCompositeKey = data;

            for (var i = 0; i < rounds; ++i)
            {
                if (i % 5000 == 0)
                {
                    percentComplete.Report(i / rounds * 100);

                }
                byteCompositeKey = cipher.ProcessBytes(byteCompositeKey);

            }
            percentComplete.Report(100);

            return byteCompositeKey;



        }

        private IBufferedCipher GetCipher(bool encrypt, byte[] key)
        {
            IBufferedCipher cipher = null;

            switch (AlgorithmType)
            {
                case CryptoAlgoritmType.AES_ECB:
                    cipher = CipherUtilities.GetCipher("AES/ECB/NOPADDING");
                    break;
                case CryptoAlgoritmType.AES_CBC_PKCS7:
                    break;
            }
            cipher.Init(encrypt, new KeyParameter(key));

            return cipher;
        }
    }
}
