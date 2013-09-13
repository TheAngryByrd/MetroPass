using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;

namespace MetroPass.WP8.Infrastructure.Cryptography
{
    public class ManagedCrypto : IEncryptionEngine
    {
        public CryptoAlgoritmType AlgorithmType
        {
            get;
            set;
        }

        public Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv) {
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

        public Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv)
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
