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
            return new SkydriveClient();
        }


    }
}