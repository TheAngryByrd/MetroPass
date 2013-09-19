using System;
using Metropass.Core.PCL.Encryption;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Threading.Tasks;

namespace MetroPass.WinRT.Infrastructure.Encryption
{
    public class BouncyCastleCrypto : IEncryptionEngine
    {      

        protected IBufferedCipher GetCipher(bool encrypt, byte[] key)
        {
            IBufferedCipher cipher = null;

            cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");                 
           
            cipher.Init(encrypt, new KeyParameter(key));

            return cipher;
        }

        public CryptoAlgoritmType AlgorithmType { get; set; }

        public async Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            IBufferedCipher cipher = GetCipher(true, key);

            return await Process(data, cipher);
        }

        public async Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            IBufferedCipher cipher = GetCipher(false, key);


            return await Process(data, cipher);
        }

        private Task<byte[]> Process(byte[] data, IBufferedCipher cipher)
        {

            return Task.Run<byte[]>(() =>
            {
                var byteCompositeKey = data;
            
                byteCompositeKey = cipher.ProcessBytes(byteCompositeKey);  

                return byteCompositeKey;

            });
        
        }

    }
}
