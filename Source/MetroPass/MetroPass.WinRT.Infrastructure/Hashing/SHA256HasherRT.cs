using Framework;
using Metropass.Core.PCL.Hashing;

namespace MetroPass.WinRT.Infrastructure.Hashing
{
    public class SHA256HasherRT : ICanSHA256Hash 
    {
        public byte[] Hash(params byte[] bytesToHash)
        {
            return Framework.SHA256Hasher.Hash(bytesToHash.AsBuffer()).AsBytes();
        }
    }
}
