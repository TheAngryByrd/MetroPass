using System.Linq;
using System.Threading.Tasks;
using Metropass.Core.PCL.PasswordGeneration;
using MetroPass.WinRT.Infrastructure.PasswordGeneration;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.W8.Integration.Tests
{
    [TestClass]
    public class GeneratePasswordTests
    {
        bool ContainsCharactersFromFormat(string format, string characterSet)
        {
            return format.Cast<char>().All(c => characterSet.Contains(c.ToString()));
        }

        bool DoesntContainCharactersFromFormat(string format, string characterSet)
        {
            return format.Cast<char>().All(c => !characterSet.Contains(c.ToString()));
        }

        [TestMethod]
        public void GeneratePasswordFromLengthAndStringTest()
        {
            const int length = 5;

            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            const string characterSet = PasswordGeneratorCharacterSets.Uppercase;
            string password = generator.GeneratePassword(length, new string[]{characterSet});

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromLengthAndStringAsyncTest()
        {
            const int length = 5;

            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            const string characterSet = PasswordGeneratorCharacterSets.Uppercase;
            string password = await generator.GeneratePasswordAsync(length, new[] { characterSet });

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, characterSet));
        }

        [TestMethod]
        public async Task GeneratePasswordFromListAsync()
        {
            var length = 5;

            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            string[] characterSet = { PasswordGeneratorCharacterSets.Uppercase, PasswordGeneratorCharacterSets.Lowercase, PasswordGeneratorCharacterSets.Digit };
            string password = await generator.GeneratePasswordAsync(length, characterSet);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, string.Join("",characterSet)));
        }

        [TestMethod]
        public async Task GeneratePasswordExcludingCharacter()
        {
            const int length = 500;
            string[] characterSet = { "AB" };
            const string excludeCharacters = "B";
            PasswordGeneratorBase generator = new PasswordGeneratorRT();
            string password = await generator.GeneratePasswordAsync(length, characterSet, excludeCharacters);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(ContainsCharactersFromFormat(password, "A"));
            Assert.IsTrue(DoesntContainCharactersFromFormat(password, "B"));
        }

    }

 
}
