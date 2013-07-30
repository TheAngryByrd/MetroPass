using System;
using MetroPass.Core.Interfaces;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Threading.Tasks;

namespace MetroPass.WinRT.Infrastructure.Encryption
{
    public class BouncyCastleCrypto : EncryptionEngineBase
    {
        public BouncyCastleCrypto(CryptoAlgoritmType algorithmType) : base(algorithmType)
        {
        }

        public async override Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
            IBufferedCipher cipher = GetCipher(true, key);

            return await Process(data, rounds, percentComplete, cipher);
        }

        public async override Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
            IBufferedCipher cipher = GetCipher(false, key);


            return await Process(data, rounds, percentComplete, cipher);
        }

        private Task<byte[]> Process(byte[] data, double rounds, IProgress<double> percentComplete, IBufferedCipher cipher)
        {

            return Task.Run<byte[]>(() =>
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

            });
        
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
