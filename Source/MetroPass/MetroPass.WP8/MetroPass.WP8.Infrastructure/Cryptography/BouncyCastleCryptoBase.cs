using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MetroPass.WP8.Infrastructure.Cryptography
{
    public abstract class BouncyCastleCryptoBase : IEncryptionEngine
    {
        public BouncyCastleCryptoBase(CryptoAlgoritmType algorithmType)
        {
            AlgorithmType = algorithmType;
        }
        public CryptoAlgoritmType AlgorithmType
        {
            get;
            set;
        }


        protected IBufferedCipher GetCipher(bool encrypt, byte[] key, byte[] iv = null)
        {
            IBufferedCipher cipher = null;

            switch (AlgorithmType)
            {
                case CryptoAlgoritmType.AES_ECB:
                    cipher = CipherUtilities.GetCipher("AES/ECB/NOPADDING");
                    break;
                case CryptoAlgoritmType.AES_CBC_PKCS7:
                    cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
                    break;
            }
            ICipherParameters param = new KeyParameter(key);
            if(iv != null)
            {
                param = new ParametersWithIV(param, iv);
            }
          
            cipher.Init(encrypt,param);
            
            return cipher;
        }

        public abstract Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);

        public abstract Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv, double rounds, IProgress<double> percentComplete);
    }
}
