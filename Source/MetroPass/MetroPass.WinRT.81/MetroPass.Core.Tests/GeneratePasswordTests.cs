using System.Threading.Tasks;
using MetroPass.WinRT.Infrastructure.PasswordGeneration;
using Metropass.Core.PCL.PasswordGeneration;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

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

            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            var characterSet = PasswordGeneratorCharacterSets.Uppercase;
            string password = generator.GeneratePassword(length, new string[] { characterSet });

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromLengthAndStringAsyncTest()
        {
            var length = 5;

            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            var characterSet = PasswordGeneratorCharacterSets.Uppercase;
            string password = await generator.GeneratePasswordAsync(length, new string[] { characterSet });

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromListAsync()
        {
            var length = 5;

            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            string[] characterSet = new string[] { PasswordGeneratorCharacterSets.Uppercase, PasswordGeneratorCharacterSets.Lowercase, PasswordGeneratorCharacterSets.Digit };
            string password = await generator.GeneratePasswordAsync(length, characterSet);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, string.Join("", characterSet)));
        }

        [TestMethod]
        public async Task GeneratePasswordExcludingCharacter()
        {
            var length = 500;
            string[] characterSet = new string[] { "AB" };
            var excludeCharacters = "B";
            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            string password = await generator.GeneratePasswordAsync(length, characterSet, excludeCharacters);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, "A"));
            Assert.IsTrue(DoesntContainCharactersFromFormat(password, "B"));
        }

    }


}
