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
                   var eas = new AesManaged
                   {
                       KeySize = 256,
                       Key = key,
                       IV = iv

                   };

                   return eas.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);

               });            
        }

        public override Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete)
        {
             return Task.Run(() =>
               {
                   var eas = new AesManaged
                   {
                       KeySize = 256,
                       Key = key,
                       IV = iv                     
                       
                   };

                  return eas.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
               });         
        }
    }
}
