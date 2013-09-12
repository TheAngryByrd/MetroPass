using System;
using System.Linq;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;
using Org.BouncyCastle.Crypto;

namespace MetroPass.WP8.Infrastructure.Cryptography
{
    public class MultiThreadedBouncyCastleCrypto : BouncyCastleCryptoBase
    {
        public MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType algorithmType)
            : base(algorithmType)
        {
        }

        public override async Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
            return await ProcessMultiThreaded(true, data, key, rounds, percentComplete);
        }

        public override async Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
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
    }
}
