using MetroPass.WP8.UI.DataModel;
using Microsoft.Live;

namespace MetroPass.WP8.UI.Utils
{
    public class Cache
    {
        private static Cache instance = new Cache();

        private Cache()
        {
        }

        public static Cache Instance { get { return instance; } }

        public string DatabaseName { get; set; }

        public LiveConnectSession SkydriveSession { get; set; }

        public string DropboxUserToken { get; set; }
        public string DropboxUserSecret { get; set; }

        public DownloadFileNavigationCache DownloadFileNavigationCache { get; set; }
    }

    public struct DownloadFileNavigationCache
    {
        public string DatabaseName { get; set; }
        public DownloadType DownloadType { get; set; }
        public string ReturnUrl { get; set; }
    }
}
