using System;
using MetroPass.WP8.UI.Services.Cloud.Skydrive;

namespace MetroPass.WP8.UI.Services.Cloud
{
    public class CloudProviderFactory : ICloudProviderFactory
    {
        public ICloudProviderAdapter GetCloudProvider(CloudProvider cloudProvider)
        {
            if (cloudProvider == CloudProvider.SkyDrive)
                return new SkyDriveClient();
            else if (cloudProvider == CloudProvider.Dropbox)
                return null;

            throw new ArgumentException("Invalid cloud provider");
        }
    }
}