using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;

namespace MetroPass.Core.Security
{
    public class PasswordGenerator
    {
        public const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Lowercase = "abcdefhijklmnopqrstuvwxyz";
        public const string Digits = "0123456789";
        public const string Minus = "-";
        public const string Underscore = "_";
        public const string Space = " ";
        public const string Special = "!@#$%^&*+=?,.";
        public const string Brackets = "(){}[]<>";

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
    
        private string ExcludeCharactersFromString(string charactersToExclude, string joinedCharacterSet)
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
    
        private int GenerateRandomNumber(int max)
        {
            return Math.Abs((int)CryptographicBuffer.GenerateRandomNumber()) % (max);
        }
    }
}