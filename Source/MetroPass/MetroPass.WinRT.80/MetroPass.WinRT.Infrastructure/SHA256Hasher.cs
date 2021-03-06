﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPass.WinRT.Infrastructure
{
    public class SHA256Hasher
    {
        public static IBuffer Hash(params IBuffer[] bufferToHash)
        {
            var hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hashProvider.CreateHash();
            foreach (var item in bufferToHash)
            {
                hash.Append(item);
            }

            var hashedBuffer = hash.GetValueAndReset();
            return hashedBuffer;
        }
    }
}
