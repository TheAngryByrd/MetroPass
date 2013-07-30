using System;
using Metropass.Core.PCL.PasswordGeneration;
using Windows.Security.Cryptography;

namespace MetroPass.WinRT.Infrastructure.PasswordGeneration
{
    public class PasswordGeneratorRT : PasswordGeneratorBase
    {
        public override int GenerateRandomNumber(int max)
        {
            return Math.Abs((int)CryptographicBuffer.GenerateRandomNumber()) % (max);
        }
    }
}