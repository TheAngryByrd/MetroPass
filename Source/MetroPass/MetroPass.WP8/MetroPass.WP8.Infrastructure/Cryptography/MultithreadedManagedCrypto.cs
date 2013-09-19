using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;

namespace MetroPass.WP8.Infrastructure.Cryptography
{
    public class MultithreadedManagedCrypto : IKeyTransformer
    {
        public async Task<byte[]> Transform(byte[] data, byte[] key, double rounds, IProgress<double> percentComplete)
        {
            var aes = new AesManaged
            {
                KeySize = 256,
                IV = new byte[16],
                Key = key,
            };

            var bData = data;

            var t1 = Task.Run(() =>
            {

                for (var i = 0; i < rounds; ++i)
                {
                    aes.CreateEncryptor().TransformBlock(bData, 0, 16, bData, 0);
                }
            });

            var t2 = Task.Run(() =>
            {
                for (var i = 0; i < rounds; ++i)
                {
                    if (i % 5000 == 0)
                    {
                        percentComplete.Report(i / rounds * 100);

                    }
                    aes.CreateEncryptor().TransformBlock(bData, 16, 16, bData, 16);
                }
            });

            await Task.WhenAll(t1, t2);

            return bData;
        }
    }
}
