using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;
using Org.BouncyCastle.Crypto;

namespace MetroPass.WP8.Infrastructure.Cryptography
{
    public class ManagedCrypto : BouncyCastleCryptoBase
    {

        public ManagedCrypto(CryptoAlgoritmType AlgorithmType)
            : base(AlgorithmType)
        {

        }
        public CryptoAlgoritmType AlgorithmType
        {
            get;
            set;
        }

        public override Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete) {
           return Task.Run(() =>
               {
                   var cipher = GetCipher(true, key, iv);


                   return cipher.ProcessBytes(data);

               });            
        }

        public override Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
             return Task.Run(() =>
               {
                   var cipher = GetCipher(false, key, iv);
                   var retval = cipher.ProcessBytes(data);

                   return retval;
               });         
        }
    }
}
