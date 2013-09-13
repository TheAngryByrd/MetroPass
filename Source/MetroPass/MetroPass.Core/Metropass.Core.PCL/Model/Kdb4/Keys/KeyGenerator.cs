using System;
using System.Linq;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Hashing;

namespace Metropass.Core.PCL.Model.Kdb4.Keys
{
    public class KeyGenerator : IKeyGenerator
    {
        private readonly ICanSHA256Hash _hasher;
        private readonly IKeyTransformer _keyTransformer; 
        private readonly IProgress<double> _progress;

        private readonly CompositeKey _compositeKey;

        public KeyGenerator(ICanSHA256Hash hasher, 
            IKeyTransformer keyTransformer, 
            CompositeKey compositeKey, 
            IProgress<double> progress)
        {             
            _hasher = hasher;
            _keyTransformer = keyTransformer;
            _compositeKey = compositeKey;
            _progress = progress;
        }

        public async Task<byte[]> GenerateHashedKeyAsync(byte[] masterSeed, byte[] transformSeed, int rounds)
        {
            var key = await GenerateKeyAsync(transformSeed, rounds);

            var aesKey = _hasher.Hash(masterSeed, key);

            return aesKey;
        }

       
        private async Task<byte[]> GenerateKeyAsync(byte[] transformSeed, int rounds)
        {
            byte[] rawCompositeKey = CreateRawCompositeKey32();

            var data = await _keyTransformer.Transform(rawCompositeKey, transformSeed, rounds, _progress);

            return _hasher.Hash(data);
        }

        private byte[] CreateRawCompositeKey32()
        {
            var keyData = _compositeKey.UserKeys.Select(uk => uk.KeyData).ToArray();
            return _hasher.Hash(keyData);
        }
    }
}
