using System;
using System.Threading.Tasks;
using Metropass.Core.PCL.PasswordGeneration;

namespace MetroPass.UI.Tests.Mocks
{
    public class MockPasswordGenerator : IPasswordGenerator
    {
        public int Length { get; set; }

        public string[] CharacterSet { get; set; }

        public string CharactersToExclude { get; set; }

        public Task<string> GeneratePasswordAsync(int length, string[] characterSet, string charactersToExclude = null)
        {
            Length = length;
            CharacterSet = characterSet;
            CharactersToExclude = charactersToExclude;

            return Task.Run<string>(() => { return "Password"; });
        }

        public string GeneratePassword(int length, string[] characterSet, string charactersToExclude = null)
        {
            throw new NotImplementedException();
        }

        public string ExcludeCharactersFromString(string charactersToExclude, string joinedCharacterSet)
        {
            throw new NotImplementedException();
        }

        public int GenerateRandomNumber(int max)
        {
            throw new NotImplementedException();
        }
    }
}