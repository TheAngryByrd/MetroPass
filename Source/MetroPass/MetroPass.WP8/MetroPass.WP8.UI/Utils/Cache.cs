using System.IO.IsolatedStorage;
using Microsoft.Live;
using Windows.Storage;

namespace MetroPass.WP8.UI.Utils
{
    public class Cache : ICache
    {
   

        private IsolatedStorageSettings _localSettings;

        public Cache()
        {
            _localSettings = IsolatedStorageSettings.ApplicationSettings;
        }

        public string DatabaseName { get; set; }
        public LiveConnectSession SkydriveSession { get; set; }
        public string DropboxUserToken
        {
            get { return _localSettings["DropboxUserToken"].ToString(); }
            set { _localSettings["DropboxUserToken"] = value; }
        }

        public string DropboxUserSecret
        {
            get { return _localSettings["DropboxUserSecret"].ToString(); }
            set { _localSettings["DropboxUserSecret"] = value; }
        }

        public DownloadFileNavigationCache DownloadFileNavigationCache { get; set; }
    }

    public struct DownloadFileNavigationCache
    {
        public string DatabaseName { get; set; }
        public DownloadType DownloadType { get; set; }
        public string ReturnUrl { get; set; }
    }
}
