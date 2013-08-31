using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MetroPass.WP8.UI.Utils;
using Microsoft.Live;

namespace MetroPass.WP8.UI.Services
{
    public class SkydriveClient : ICloudProviderAdapter
    {
        private LiveConnectClient _liveClient;

        public SkydriveClient()
        {
            _liveClient = new LiveConnectClient(Cache.Instance.SkydriveSession);
        }

        public async Task<IEnumerable<ICloudItem>> GetItems(string path)
        {
            LiveOperationResult operationResult = await _liveClient.GetAsync(path + "/files");
            return TryLoadItems(operationResult);
        }

        public async Task<Stream> DownloadItem(string path)
        {
            var operationResult= await _liveClient.DownloadAsync(path + "/content");
            return operationResult.Stream;
        }

        private IEnumerable<ICloudItem> TryLoadItems(LiveOperationResult operationResult)
        {
            dynamic result = operationResult.Result;

            if (result.data == null) return new List<SkyDriveItem>();            

            dynamic items = result.data;
            return LoadItems(items);
        }

        private IEnumerable<ICloudItem> LoadItems(dynamic items)
        {
            var resultList = new List<SkyDriveItem>();
            foreach (dynamic item in items)
            {
                resultList.Add(new SkyDriveItem(item));
            }

            return resultList;
        }
    }
}