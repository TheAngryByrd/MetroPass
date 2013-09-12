using System.Threading.Tasks;

namespace Metropass.Core.PCL.Model.Kdb4.Keys
{
    public interface IKeyGenerator
    {
        Task<byte[]> GenerateHashedKeyAsync(byte[] masterSeed, byte[] transformSeed, int rounds);
    }
}
