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
            var characterSet = generator.Uppercase;
            string password = generator.GeneratePassword(characterSet, length);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(FormatValid(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromLengthAndStringAsyncTest()
        {
            var length = 5;

            PasswordGenerator generator = new PasswordGenerator();
            var characterSet = generator.Uppercase;
            string password = await generator.GeneratePasswordAsync(characterSet, length);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(FormatValid(password, characterSet));
        }




    }

    public class PasswordGenerator
    {
        public string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string Lowercase = "abcdefhijklmnopqrstuvwxyz";
        public string Digits = "0123456789";
        public string Minus = "-";
        public string Underscore = "_";
        public string Space = " ";
        public string Special = "!@#$%^&*+=?,.";
        public string Brackets = "(){}[]<>";

        public Task<string> GeneratePasswordAsync(string characterSet, int length)
        {
            return Task.Run<string>(() => {
                return GeneratePassword(characterSet, length);
            }); 
        }

        public string GeneratePassword(string characterSet, int length)
        {
            string retVal = string.Empty;
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                var nextIndex = GenerateRandomNumber(characterSet.Length);
                retVal += characterSet[nextIndex];
            }
            return retVal;
        }
  
        private int GenerateRandomNumber(int max)
        {
            return Math.Abs((int)CryptographicBuffer.GenerateRandomNumber()) % (max);
        }
    }
}
