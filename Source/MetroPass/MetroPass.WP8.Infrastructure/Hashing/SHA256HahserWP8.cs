using System;
using System.IO;
using System.Security.Cryptography;
using Metropass.Core.PCL.Hashing;

namespace MetroPass.WP8.Infrastructure.Hashing
{
    public class SHA256HahserWP8 : ICanSHA256Hash
    {
        public byte[] Hash(params byte[][] bytesToHash) {
            using(var hash = new SHA256Managed())
            using (var stream = new MemoryStream())
            {
                foreach (var item in bytesToHash)
                {
                    stream.Write(item, 0, item.Length);
                }
                return hash.ComputeHash(stream.ToArray());
            }    
        }
    }
}
