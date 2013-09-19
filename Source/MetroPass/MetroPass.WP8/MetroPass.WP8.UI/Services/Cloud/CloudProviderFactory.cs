using System;
using System.Collections.Generic;
using MetroPass.WP8.UI.Services.Cloud.Skydrive;
using MetroPass.WP8.UI.Services.Cloud.Dropbox;
using MetroPass.WP8.UI.Utils;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI.Services.Cloud
{
    public class CloudProviderFactory : ICloudProviderFactory
    {
        private readonly ICache _cache;

        private Dictionary<string, CloudProvider> enums = new Dictionary<string, CloudProvider>
        {
            { CloudProvider.Dropbox.ToString(), CloudProvider.Dropbox },
            { CloudProvider.SkyDrive.ToString(), CloudProvider.SkyDrive }
        };

        public CloudProviderFactory(ICache cache)
        {
            _cache = cache;
        }


        public Task<ICloudProviderAdapter> GetCloudProvider(string cloudProvider)
        {
            return GetCloudProvider(enums[cloudProvider]);
        }

        public async Task<ICloudProviderAdapter> GetCloudProvider(CloudProvider cloudProvider)
        {
            ICloudProviderAdapter retVal = null;

            if (cloudProvider == CloudProvider.SkyDrive)
                retVal = new SkyDriveClient(_cache);
            else if (cloudProvider == CloudProvider.Dropbox)
                retVal = new DropboxClient(_cache);
            else
                 throw new ArgumentException("Invalid cloud provider");
            

            await retVal.Activate();
            return retVal;
        }
    }
}