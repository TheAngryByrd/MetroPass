using System;
using System.Collections.Generic;
using System.IO;
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
                   byte[] encrypted;
                   using (AesManaged aesAlg = new AesManaged())
                   {
                       aesAlg.Key = key;
                       aesAlg.IV = iv;

                       // Create a decrytor to perform the stream transform.
                       ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                       using (MemoryStream msEncrypt = new MemoryStream())
                       {
                           using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                           {
                               using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                               {
                                   //Write all data to the stream.
                                   swEncrypt.Write(data);
                               }
                               encrypted = msEncrypt.ToArray();
                           }
                       }
                   }
                   return encrypted;

               });            
        }

        public Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete) {
             return Task.Run(() =>
               {
                   byte[] decrypted =  null;
                   using (AesManaged aesAlg = new AesManaged())
                   {
                       aesAlg.Key = key;
                       aesAlg.IV = iv;

                       // Create a decrytor to perform the stream transform.
                       ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                       using (MemoryStream msDecrypt = new MemoryStream(data))
                       {
                           using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                           {
                               decrypted = csDecrypt.ToArray();
                           }
                       }

                   }

                   return decrypted;
               });         
        }
    }
}
