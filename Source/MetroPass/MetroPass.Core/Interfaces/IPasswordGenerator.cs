using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;

namespace MetroPass.Core.Interfaces
{
    public interface IPasswordGenerator
    {
        Task<string> GeneratePasswordAsync(int length, string[] characterSet, string charactersToExclude = null);

        string GeneratePassword(int length, string[] characterSet, string charactersToExclude = null);

        string ExcludeCharactersFromString(string charactersToExclude, string joinedCharacterSet);

        int GenerateRandomNumber(int max);
    }
}