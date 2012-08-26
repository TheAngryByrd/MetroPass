using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPassLib.Helpers
{
    public class SHA256Hasher
    {
        public static IBuffer Hash(IBuffer bufferToHash)
        {
            var hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hashProvider.CreateHash();
            hash.Append(bufferToHash);
            var hashedBuffer = hash.GetValueAndReset();
            return hashedBuffer;
        }
    }
}
