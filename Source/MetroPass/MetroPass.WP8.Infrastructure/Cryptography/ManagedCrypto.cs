using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;

namespace MetroPass.WP8.Infrastructure.Cryptography
{
    public class ManagedCrypto : IEncryptionEngine
    {

        public ManagedCrypto(CryptoAlgoritmType AlgorithmType)
        {

        }
        public CryptoAlgoritmType AlgorithmType
        {
            get;
            set;
        }

        public Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete) {
           return Task.Run(() =>
               {
                   var aes = new AesManaged
                    {
                        KeySize = 256,
                        IV = new byte[16],
                        Key = key,
                    };

                   var block = new byte[data.Length];

                    Buffer.BlockCopy(data,0,block,0,data.Length);

                    for (var i = 1; i <= rounds; i++)
                    {
                        aes.CreateEncryptor().TransformBlock(
                            block, 0, 16, block, 0);
                        aes.CreateEncryptor().TransformBlock(
                            block, 16, 16, block, 16);
                    }

                    return block;
               });            
        }

        public Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete) {
             return Task.Run(() =>
               {
                   var aes = new AesManaged
                    {
                        KeySize = 256,
                        IV = new byte[16],
                        Key = key,
                    };

                   var block = new byte[data.Length];

                    Buffer.BlockCopy(data,0,block,0,data.Length);

                    for (var i = 1; i <= rounds; i++)
                    {
                        aes.CreateEncryptor().TransformBlock(
                            block, 0, 16, block, 0);
                        aes.CreateEncryptor().TransformBlock(
                            block, 16, 16, block, 16);
                    }

                    return block;
               });         
        }
    }
}
