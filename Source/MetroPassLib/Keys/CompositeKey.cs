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

        public CompositeKey()
        {
            UserKeys = new List<IUserKey>();
        }

        //public static bool TransformKeyManaged(byte[] pbNewKey32, byte[] pbKeySeed32,
        //    ulong uNumRounds)
        //{
        //    var newKeyBuffer = CryptographicBuffer.CreateFromByteArray(pbNewKey32);

        //    SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
        //    var symetricKey = symKeyProvider.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(pbKeySeed32));

            
        //    byte[] pbIV = new byte[16];
        //    Array.Clear(pbIV, 0, pbIV.Length);
        //    var iv = CryptographicBuffer.CreateFromByteArray(pbIV);

        //    bool success = TransformKeyManaged(newKeyBuffer, symetricKey, iv, uNumRounds);
        //    CryptographicBuffer.CopyToByteArray(newKeyBuffer, out pbNewKey32);
        //    return success;
        //}

        public static IBuffer TransformKeyManaged(IBuffer rawCompositeKey, CryptographicKey transFormKey, IBuffer iv, ulong rounds)
        {
           
                for (ulong i = 0; i < rounds; ++i)
                {
                    rawCompositeKey = CryptographicEngine.Decrypt(transFormKey, rawCompositeKey, iv);
                }
                return rawCompositeKey;
         
        }

        public static Task<IBuffer> TransformKeyManagedAsync(IBuffer rawCompositeKey, CryptographicKey transFormKey, IBuffer iv, ulong rounds) { 
            return Task.Run( () => 
            {
                return TransformKeyManaged(rawCompositeKey, transFormKey, iv, rounds);
            });
        }

        public List<IUserKey> UserKeys { get; set; }

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
