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

        public DropboxClient()
        {
            _client = new DropNetClient(
              ApiKeys.DropBoxKey, 
              ApiKeys.DropBoxSecret,
              Cache.Instance.DropboxUserToken,
              Cache.Instance.DropboxUserSecret);
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
