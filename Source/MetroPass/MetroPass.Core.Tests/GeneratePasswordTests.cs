using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.Core.Tests
{
    [TestClass]
    public class GeneratePasswordTests
    {
        [TestMethod]
        public void GeneratePasswordFromLengthAndStringTest()
        {
            var length = 90;
            var characterSet = "ABCDEFG";

            string password = GeneratePassword(characterSet, length);

            Assert.AreEqual(length, password.Length);
            Assert.IsTrue(FormatValid(password, characterSet));

        }

        private string GeneratePassword(string characterSet, int length)
        {
            string retVal = string.Empty;
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                var nextIndex = random.Next(0, characterSet.Length - 1);
                retVal += characterSet[nextIndex];
            }
            return retVal;
        }

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
    }
}
