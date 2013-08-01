//using System.Linq;
//using Framework;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MetroPass.Core.Interfaces;
//using MetroPass.WinRT.Infrastructure.Encryption;
//using Metropass.Core.PCL.Model.Kdb4.Keys;
//using Windows.Security.Cryptography.Core;
//using Windows.Storage.Streams;

//namespace MetroPass.Core.Model.Keys
//{
//    public class CompositeKey
//    {
//        public IProgress<double> PercentComplete { get; set; }
//        public IList<IUserKey> UserKeys { get; set; }

//        public CompositeKey(IList<IUserKey> userKeys)
//        {
//            Init(userKeys, new NullableProgress<double>());
//        }

//        public CompositeKey(IList<IUserKey> userKeys, IProgress<double> percentComplete)
//        {
//            Init(userKeys, percentComplete);            
//        }

//        private void Init(IList<IUserKey> userKeys, IProgress<double> percentComplete)
//        {
//            UserKeys = userKeys;
//            PercentComplete = percentComplete;
//        }

//        public async Task<IBuffer> GenerateHashedKeyAsync(IBuffer masterSeed, IBuffer transformSeed, ulong rounds)
//        {
//            var key = await GenerateKeyAsync(transformSeed, rounds);

//            var aesKey = SHA256Hasher.Hash(masterSeed, key);

//            return aesKey;
//        }

//        private async Task<IBuffer> GenerateKeyAsync(IBuffer transformSeed, ulong rounds)
//        {
//            IBuffer rawCompositeKey = CreateRawCompositeKey32();

//            var cryptoEngine = new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB);

//            var data = await cryptoEngine.Encrypt(rawCompositeKey.AsBytes(), transformSeed.AsBytes(), null, (int)rounds, PercentComplete);
            
//            return SHA256Hasher.Hash(data.AsBuffer());
//        }              

//        private IBuffer CreateRawCompositeKey32()
//        {
//            return SHA256Hasher.Hash(UserKeys.Select(uk => uk.KeyData.AsBuffer()).ToArray());
//        }
//    }
//}
