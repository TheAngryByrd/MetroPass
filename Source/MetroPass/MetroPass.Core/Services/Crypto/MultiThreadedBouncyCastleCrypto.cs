using System;
using System.Linq;
using System.Threading;
using Framework;
using MetroPass.Core.Interfaces;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Windows.Storage.Streams;
using System.Threading.Tasks;

namespace MetroPass.Core.Services.Crypto
{
    public class MultiThreadedBouncyCastleCrypto : EncryptionEngine
    {
        public MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType algorithmType)
            : base(algorithmType)
        {
        }

        public async override Task<IBuffer> Encrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete)
        {
            return await ProcessMultiThreaded(true, data, key, rounds, percentComplete);
        }

        private async Task<IBuffer> ProcessMultiThreaded(bool encrypt, IBuffer data, IBuffer key, double rounds, IProgress<double> percentComplete)
        {
            var bData = data.AsBytes();
            var bKey = key.AsBytes();

            var t1 = Task.Run(() =>
            {
                IBufferedCipher cipher = GetCipher(encrypt, key);

                return Process(bData.Take(16).ToArray().AsBuffer(), rounds, percentComplete, cipher);
            });
            var t2 = Task.Run(() =>
            {
                IBufferedCipher cipher = GetCipher(encrypt, key);

                return Process(bData.Skip(16).Take(16).ToArray().AsBuffer(), rounds, percentComplete, cipher);
            });

            await Task.WhenAll(t1, t2);

            return t1.Result.AsBytes().Concat(t2.Result.AsBytes()).ToArray().AsBuffer();
        }

        public async override Task<IBuffer> Decrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete)
        {
            return await ProcessMultiThreaded(false, data, key, rounds, percentComplete);
        }


        private IBuffer Process(IBuffer data, double rounds, IProgress<double> percentComplete, IBufferedCipher cipher)
        {


            var byteCompositeKey = data.AsBytes();

            for (var i = 0; i < rounds; ++i)
            {
                if (i % 5000 == 0)
                {
                    percentComplete.Report(i / rounds * 100);

                }
                byteCompositeKey = cipher.ProcessBytes(byteCompositeKey);

            }
            percentComplete.Report(100);

            return byteCompositeKey.AsBuffer();



        }

        private IBufferedCipher GetCipher(bool encrypt, IBuffer key)
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
            cipher.Init(encrypt, new KeyParameter(key.AsBytes()));

            return cipher;
        }
    }
}
