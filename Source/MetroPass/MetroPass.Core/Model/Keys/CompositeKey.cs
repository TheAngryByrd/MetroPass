using MetroPass.Core.Helpers;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Services.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPass.Core.Model.Keys
{
    public class CompositeKey
    {
        public IProgress<double> PercentComplete { get; set; }
        public CompositeKey(IList<IUserKey> userKeys)
        {
            Init(userKeys, new NullableProgress<double>());
        }
        public CompositeKey(IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            Init(userKeys, percentComplete);
            
        }

        public void Init(IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            UserKeys = userKeys;
            PercentComplete = percentComplete;
        }

        public async Task<IBuffer> GenerateHashedKeyAsync(IBuffer masterSeed, IBuffer transformSeed, ulong rounds)
        {
            var key = await GenerateKeyAsync(transformSeed, rounds);

            var aesKey = SHA256Hasher.Hash(masterSeed, key);

            return aesKey;
        }

        public async Task<IBuffer> GenerateKeyAsync(IBuffer transformSeed, ulong rounds)
        {
            IBuffer rawCompositeKey = await CreateRawCompositeKey32();

            var cryptoEngine = new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB);

            var data = await cryptoEngine.Encrypt(rawCompositeKey, transformSeed, null, (int)rounds, PercentComplete);
            
            return SHA256Hasher.Hash(data);
        }


        public async Task<IBuffer> TransformKeyAsync(IBuffer rawCompositeKey, IBuffer transformSeed, ulong rounds)
        {       
            var data = await Task.Run(() =>
                {
                    var cipher = CipherUtilities.GetCipher("AES/ECB/NOPADDING");
                    cipher.Init(true, new KeyParameter(transformSeed.AsBytes()));
                    var byteCompositeKey = rawCompositeKey.AsBytes();
                    var roundsInDouble = (double)rounds;
                    for (var i = 0; i < roundsInDouble; ++i)
                    {
                        if (i % 1000 == 0)
                        {
                            PercentComplete.Report(i / roundsInDouble * 100);

                        }

                        byteCompositeKey = cipher.ProcessBytes(byteCompositeKey);

                    }
                    return byteCompositeKey.AsBuffer();
                });

            return SHA256Hasher.Hash(data);
        }

        public async Task<IBuffer> TransformKeyAsync(IBuffer rawCompositeKey, CryptographicKey transFormKey, ulong rounds)
        {
            var transformedCompositeKey = await TransformKeyManagedAsync(rawCompositeKey, transFormKey, null, rounds);
            return SHA256Hasher.Hash(transformedCompositeKey);
        }



        private IBuffer TransformKeyManaged(IBuffer rawCompositeKey, CryptographicKey transFormKey, IBuffer iv, ulong rounds)
        {
            
            var roundsInDouble = (double)rounds;
            for (var i = 0; i < roundsInDouble; ++i)
            {
                if (i % 1000 == 0)
                {
                    PercentComplete.Report(i / roundsInDouble * 100);

                }

                rawCompositeKey = CryptographicEngine.Encrypt(transFormKey, rawCompositeKey, iv);

            }
            PercentComplete.Report(100);
            return rawCompositeKey;
        }

        public Task<IBuffer> TransformKeyManagedAsync(IBuffer rawCompositeKey, CryptographicKey transFormKey, IBuffer iv, ulong rounds)
        {
            return Task.Run(() =>
            {
                return TransformKeyManaged(rawCompositeKey, transFormKey, iv, rounds);
            });
        }

        public IList<IUserKey> UserKeys { get; set; }

        /// <summary>
        /// Creates the composite key from the supplied user key sources (password,
        /// key file, user account, computer ID, etc.).
        /// </summary>
        public async Task<IBuffer> CreateRawCompositeKey32()
        {
            var x = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = x.CreateHash();

            // Concatenate user key data
            foreach (IUserKey pKey in UserKeys)
            {
                IBuffer b = pKey.KeyData;
                if (b != null)
                {
                    var pbKeyData = pKey.KeyData;
                    hash.Append(pbKeyData);
                    pbKeyData = null;
                }
            }

            return hash.GetValueAndReset();
        }
    }
}
