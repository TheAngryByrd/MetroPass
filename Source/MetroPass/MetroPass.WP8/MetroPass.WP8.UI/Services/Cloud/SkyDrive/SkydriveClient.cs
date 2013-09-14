using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MetroPass.WP8.UI.Utils;
using Microsoft.Live;

namespace MetroPass.WP8.UI.Services.Cloud.Skydrive
{
    public class SkyDriveClient : ICloudProviderAdapter
    {
        private LiveConnectClient _liveClient;
        private readonly ICache _cache;

        public SkyDriveClient(ICache cache)
        {
            _cache = cache;
        }

        public async Task Activate()
        {
            var session = _cache.SkydriveSession;
            if (session == null)
            {
                var authClient = new LiveAuthClient(ApiKeys.SkyDriveClientId);
                var x = await authClient.InitializeAsync();
                session = x.Session;
            }
            _liveClient = new LiveConnectClient(session);
        }

        private IEnumerable<string> ParseScopeString(string scopesString)
        {
            return new List<string>(scopesString.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries));
        }

        public async Task Upload(string path, string fileName, Stream file)
        {
            await _liveClient.UploadAsync(path, fileName, file, OverwriteOption.Overwrite);
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