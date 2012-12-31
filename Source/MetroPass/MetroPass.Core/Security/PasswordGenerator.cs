using System;
using System.Threading.Tasks;
using MetroPass.Core.Interfaces;
using Windows.Security.Cryptography;

namespace MetroPass.Core.Security
{
    public class PasswordGenerator : IPasswordGenerator
    {
      

        public Task<string> GeneratePasswordAsync(int length, string[] characterSet, string charactersToExclude = null)
        {
            return Task.Run<string>(() =>
            {
                return GeneratePassword(length, characterSet, charactersToExclude);
            }); 
        }

        public string GeneratePassword(int length, string[] characterSet, string charactersToExclude = null)
        {
            var joinedCharacterSet = string.Join("", characterSet);

            joinedCharacterSet = ExcludeCharactersFromString(charactersToExclude, joinedCharacterSet);

            string retVal = string.Empty;
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                var nextIndex = GenerateRandomNumber(joinedCharacterSet.Length);
                retVal += joinedCharacterSet[nextIndex];
            }
            return retVal;
        }
    
        public string ExcludeCharactersFromString(string charactersToExclude, string joinedCharacterSet)
        {
            if (!string.IsNullOrEmpty(charactersToExclude))
            {
                foreach (var item in charactersToExclude)
                {
                    joinedCharacterSet = joinedCharacterSet.Replace(item.ToString(), string.Empty);
                }
            }
            return joinedCharacterSet;
        }
    
        public int GenerateRandomNumber(int max)
        {
            return Math.Abs((int)CryptographicBuffer.GenerateRandomNumber()) % (max);
        }
    }
}