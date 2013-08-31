using System;

namespace MetroPass.WP8.UI.Services
{
    public enum CloudProvider
    {
        Dropbox,
        SkyDrive
    }
    public class CloudProviderFactory : ICloudProviderFactory
    {

        public ICloudProviderAdapter GetCloudProvider(CloudProvider cloudProvider)
        {
            if (cloudProvider == CloudProvider.SkyDrive)
                return new SkydriveClient();
            else if (cloudProvider == CloudProvider.Dropbox)
                return null;

            throw new ArgumentException("Invalid cloud provider");
        }


    }
}