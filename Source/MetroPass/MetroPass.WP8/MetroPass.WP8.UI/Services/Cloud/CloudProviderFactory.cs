using System;
using MetroPass.WP8.UI.Services.Cloud.Skydrive;
using MetroPass.WP8.UI.Services.Cloud.Dropbox;
using MetroPass.WP8.UI.Utils;

namespace MetroPass.WP8.UI.Services.Cloud
{
    public class CloudProviderFactory : ICloudProviderFactory
    {
        private readonly ICache _cache;

        public CloudProviderFactory(ICache cache)
        {
            _cache = cache;
        }

        public ICloudProviderAdapter GetCloudProvider(CloudProvider cloudProvider)
        {
            if (cloudProvider == CloudProvider.SkyDrive)
                return new SkyDriveClient(_cache);
            else if (cloudProvider == CloudProvider.Dropbox)
                return new DropboxClient(_cache);

            throw new ArgumentException("Invalid cloud provider");
        }
    }
}