using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.Core.Security;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Security.Cryptography;

namespace MetroPass.Core.Tests
{
    [TestClass]
    public class GeneratePasswordTests
    {
        bool ContainsCharactersFromFormat(string format, string characterSet)
        { 
            foreach (char c in format)
            {
                if (!characterSet.Contains(c.ToString()))
                    return false;
            }
            return true;
        }

        bool DoesntContainCharactersFromFormat(string format, string characterSet)
        {
            foreach (char c in format)
            {
                if (characterSet.Contains(c.ToString()))
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
            string password = generator.GeneratePassword(length, new string[]{characterSet});

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromLengthAndStringAsyncTest()
        {
            var length = 5;

            PasswordGenerator generator = new PasswordGenerator();
            var characterSet = PasswordGenerator.Uppercase;
            string password = await generator.GeneratePasswordAsync(length, new string[] { characterSet });

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromListAsync()
        {
            var length = 5;

            PasswordGenerator generator = new PasswordGenerator();
            string[] characterSet = new string[]{PasswordGenerator.Uppercase, PasswordGenerator.Lowercase, PasswordGenerator.Digits};
            string password = await generator.GeneratePasswordAsync(length, characterSet);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, string.Join("",characterSet)));
        }

        [TestMethod]
        public async Task GeneratePasswordExcludingCharacter()
        {
            var length = 500;
            string[] characterSet = new string[] { "AB" };
            var excludeCharacters = "B";
            PasswordGenerator generator = new PasswordGenerator();
            string password = await generator.GeneratePasswordAsync(length, characterSet, excludeCharacters);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, "A"));
            Assert.IsTrue(DoesntContainCharactersFromFormat(password, "B"));
        }

    }

 
}
