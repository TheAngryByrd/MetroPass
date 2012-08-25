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

namespace MetroPassLib
{
    public class CompositeKey
    {

        public CompositeKey()
        {
            UserKeys = new List<IUserKey>();
        }

        public static bool TransformKeyManaged(byte[] pbNewKey32, byte[] pbKeySeed32,
            ulong uNumRounds)
        {
            byte[] pbIV = new byte[16];
            Array.Clear(pbIV, 0, pbIV.Length);

            SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);

            
            var symetricKey = symKeyProvider.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(pbKeySeed32));
            
            var iv = CryptographicBuffer.CreateFromByteArray(pbIV);
            
            

            return true;
        }
        public List<IUserKey> UserKeys { get; set; }

        /// <summary>
        /// Creates the composite key from the supplied user key sources (password,
        /// key file, user account, computer ID, etc.).
        /// </summary>
        private async Task<IBuffer> CreateRawCompositeKey32()
        {

            var x = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = x.CreateHash();

            // Concatenate user key data
            foreach (IUserKey pKey in UserKeys)
            {
                ProtectedBuffer b = pKey.KeyData;
                if (b != null)
                {                    
                    var pbKeyData = await b.UnProtectAsync();
                    hash.Append(pbKeyData);
                    pbKeyData = null;
                }
            }

            return hash.GetValueAndReset();
        }

    }
}
