namespace MetroPass.WP8.UI.Services
{
    public interface ICloudProviderFactory
    {
        ICloudProviderAdapter GetCloudProvider(CloudProvider cloudProvider);
    }
}