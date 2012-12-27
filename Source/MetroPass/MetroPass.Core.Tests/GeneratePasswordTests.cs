using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Security.Cryptography;

namespace MetroPass.Core.Tests
{
    [TestClass]
    public class GeneratePasswordTests
    {
        bool FormatValid(string format, string characterSet)
        { 
            foreach (char c in format)
            {
                // This is using String.Contains for .NET 2 compat.,
                //   hence the requirement for ToString()
                if (!characterSet.Contains(c.ToString()))
                    return false;
            }
            return true;
        }
  
        [TestMethod]
        public void GeneratePasswordFromLengthAndStringTest()
        {
            var length = 5;
         
            PasswordGenerator generator = new PasswordGenerator();
            var characterSet = PasswordGenerator.Uppercase;
            string password = generator.GeneratePassword(length, characterSet: characterSet);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(FormatValid(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromLengthAndStringAsyncTest()
        {
            var length = 5;

            PasswordGenerator generator = new PasswordGenerator();
            var characterSet = PasswordGenerator.Uppercase;
            string password = await generator.GeneratePasswordAsync(length, characterSet);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(FormatValid(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromListAsync()
        {
            var length = 5;

            PasswordGenerator generator = new PasswordGenerator();
            string[] characterSet = new string[]{PasswordGenerator.Uppercase, PasswordGenerator.Lowercase, PasswordGenerator.Digits};
            string password = await generator.GeneratePasswordAsync(length, characterSet);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(FormatValid(password, string.Join("",characterSet)));
        }

    }

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

        
        public Task<string> GeneratePasswordAsync(int length, params string[] characterSet)
        {
            return Task.Run<string>(() =>
            {
                return GeneratePassword(length: length, characterSet:characterSet);
            }); 
        }            
        
        public string GeneratePassword(int length, string charactersToExclude = null, params string[] characterSet)
        {
            var joinedCharacterSet = string.Join("", characterSet);

            if(!string.IsNullOrEmpty(charactersToExclude)){

                foreach (var item in charactersToExclude)
	            {
		            joinedCharacterSet.Replace(item.ToString(),string.Empty);
	            }              
            }

            string retVal = string.Empty;
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                var nextIndex = GenerateRandomNumber(joinedCharacterSet.Length);
                retVal += joinedCharacterSet[nextIndex];
            }
            return retVal;
        }
  
        private int GenerateRandomNumber(int max)
        {
            return Math.Abs((int)CryptographicBuffer.GenerateRandomNumber()) % (max);
        }
    }
}
