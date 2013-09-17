using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropNetRT;
using DropNetRT.Models;
using MetroPass.WP8.UI.Utils;

namespace MetroPass.WP8.UI.Services.Cloud.Dropbox
{
    public class DropboxClient : ICloudProviderAdapter
    {
        private DropNetClient _client;

        private readonly ICache _cache;

        public DropboxClient(ICache cache)
        { 
            _cache = cache;
        }

        public async Task Activate()
        {
            _client = new DropNetClient(
               ApiKeys.DropBoxKey,
               ApiKeys.DropBoxSecret,
               _cache.DropboxUserToken,
               _cache.DropboxUserSecret);
            _client.UseSandbox = true;
        }

        public async Task Upload(string path, string fileName, Stream file)
        {
            await _client.Upload(path, fileName, file);
        }

        public async Task<IEnumerable<ICloudItem>> GetItems(string path)
        {
            var data = await _client.GetMetaData(path);
            return LoadItems(data);         
        }

        private IEnumerable<ICloudItem> LoadItems(MetaData data)
        {
            var items = new List<DropboxItem>();
            foreach(var item in data.Contents)
            {
                items.Add(new DropboxItem(item.Path,item.Name,item.Icon));
            }

            return items;
        }

        public async Task<Stream> DownloadItem(string path)
        {            
            var bytes = await _client.GetFile(path);
            return new MemoryStream(bytes);
        }
    }
}
