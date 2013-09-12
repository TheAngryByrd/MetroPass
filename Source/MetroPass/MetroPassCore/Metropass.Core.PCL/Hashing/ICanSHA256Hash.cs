namespace Metropass.Core.PCL.Hashing
{
    public interface ICanSHA256Hash
    {    
        byte[] Hash(params byte[][] bytesToHash);
    }
}
