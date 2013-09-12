namespace MetroPass.WP8.UI.Services.Cloud
{
    public interface ICloudProviderFactory
    {
        ICloudProviderAdapter GetCloudProvider(CloudProvider cloudProvider);
    }
}