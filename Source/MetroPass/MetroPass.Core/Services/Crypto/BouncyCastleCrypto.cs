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
    public class BouncyCastleCrypto : EncryptionEngine
    {
        public BouncyCastleCrypto(CryptoAlgoritmType algorithmType) : base(algorithmType)
        {
        }

        public async override Task<IBuffer> Encrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete)
        {
            IBufferedCipher cipher = GetCipher(true, key);
            
            return await Process(data, rounds, percentComplete, cipher);
        }


        public async override Task<IBuffer> Decrypt(IBuffer data, IBuffer key, IBuffer iv, double rounds, IProgress<double> percentComplete)
        {
            IBufferedCipher cipher = GetCipher(false, key);      
          

            return await Process(data, rounds, percentComplete, cipher);
        }


        private Task<IBuffer> Process(IBuffer data, double rounds, IProgress<double> percentComplete, IBufferedCipher cipher)
        {             

            return Task.Run<IBuffer>(() =>
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

            });
        
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
