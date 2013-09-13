using System;
using System.Linq;
using Metropass.Core.PCL.Encryption;
using Org.BouncyCastle.Crypto;
using System.Threading.Tasks;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

namespace MetroPass.WinRT.Infrastructure.Encryption
{
    public class MultiThreadedBouncyCastleCrypto : IKeyTransformer
    {

        public async Task<byte[]> Transform(byte[] data, byte[] key, double rounds, IProgress<double> percentComplete)
        {
            return await ProcessMultiThreaded(true, data, key, rounds, percentComplete);
        }

        private IBufferedCipher GetCipher(bool encrypt, byte[] key)
        {
            IBufferedCipher cipher = null;         
            cipher = CipherUtilities.GetCipher("AES/ECB/NOPADDING");
     
            cipher.Init(encrypt, new KeyParameter(key));

            return cipher;
        }


        private async Task<byte[]> ProcessMultiThreaded(bool encrypt, byte[] data, byte[] key, double rounds, IProgress<double> percentComplete)
        {
            var bData = data;
            var bKey = key;

            var t1 = Task.Run(() =>
            {
                IBufferedCipher cipher = GetCipher(encrypt, key);

                return Process(bData.Take(16).ToArray(), rounds, percentComplete, cipher, true);
            });
            var t2 = Task.Run(() =>
            {
                IBufferedCipher cipher = GetCipher(encrypt, key);

                return Process(bData.Skip(16).Take(16).ToArray(), rounds, percentComplete, cipher);
            });

            await Task.WhenAll(t1, t2);

            return t1.Result.Concat(t2.Result).ToArray();
        } 

        private byte[] Process(byte[] data, double rounds, IProgress<double> percentComplete, IBufferedCipher cipher, bool track = false)
        {
            var byteCompositeKey = data;

            for (var i = 0; i < rounds; ++i)
            {                
                if (track && i % 5000 == 0)
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
