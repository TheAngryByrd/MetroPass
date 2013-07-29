namespace Metropass.Core.PCL.Hashing
{
    public interface ICan256Hash
    {
        byte[] Hash(params byte[] bytesToHash);
    }
}
