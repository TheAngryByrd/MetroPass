using System.Threading.Tasks;

namespace Metropass.Core.PCL.PasswordGeneration
{
    public interface IPasswordGenerator
    {
        Task<string> GeneratePasswordAsync(int length, string[] characterSet, string charactersToExclude = null);

        string GeneratePassword(int length, string[] characterSet, string charactersToExclude = null);

        string ExcludeCharactersFromString(string charactersToExclude, string joinedCharacterSet);

        int GenerateRandomNumber(int max);
    }
}