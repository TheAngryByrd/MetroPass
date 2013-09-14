using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI.Services.Cloud
{
    public interface ICloudProviderAdapter
    {
        Task<IEnumerable<ICloudItem>> GetItems(string path);
        Task<Stream> DownloadItem(string path);
        Task Upload(string path, string fileName, Stream file);
    }
}