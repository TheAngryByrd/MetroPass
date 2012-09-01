﻿using MetroPassLib.Helpers;
using MetroPassLib.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPassLib.Keys

{
    public class CompositeKey
    {
        public IProgress<double> PercentComplete { get; set; }
        public CompositeKey()
        {
            UserKeys = new List<IUserKey>();
            PercentComplete = new NullableProgress<double>();
        }

        public async Task<IBuffer> GenerateKeyAsync(IBuffer transformSeed, ulong rounds)
        {
            IBuffer rawCompositeKey = await CreateRawCompositeKey32();

            SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var transformSeedKey = symKeyProvider.CreateSymmetricKey(transformSeed);
            var transformedMasterKey = await TransformKeyAsync(rawCompositeKey, transformSeedKey, rounds);


            return transformedMasterKey;
        }

        public  async Task<IBuffer> TransformKeyAsync(IBuffer rawCompositeKey, CryptographicKey transFormKey, ulong rounds)
        {
            var transformedCompositeKey = await TransformKeyManagedAsync(rawCompositeKey, transFormKey, null, rounds);
            return SHA256Hasher.Hash(transformedCompositeKey);
        }

        public  IBuffer TransformKeyManaged(IBuffer rawCompositeKey, CryptographicKey transFormKey, IBuffer iv, ulong rounds)
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

        public  Task<IBuffer> TransformKeyManagedAsync(IBuffer rawCompositeKey, CryptographicKey transFormKey, IBuffer iv, ulong rounds) { 
            return Task.Run( () => 
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
