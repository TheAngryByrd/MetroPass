using System.Threading.Tasks;
namespace MetroPass.WP8.UI.Services.Cloud
{
    public interface ICloudProviderFactory
    {
        Task<ICloudProviderAdapter> GetCloudProvider(CloudProvider cloudProvider);
        Task<ICloudProviderAdapter> GetCloudProvider(string cloudProvider);
    }
}