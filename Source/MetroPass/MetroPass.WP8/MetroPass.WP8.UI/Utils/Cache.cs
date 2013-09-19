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
            get
            {
                if (!_localSettings.Contains("DropboxUserToken"))
                    return null;
                return  _localSettings["DropboxUserToken"].ToString();                    
            }
            set
            {
                _localSettings["DropboxUserToken"] = value;
                _localSettings.Save();
            }
        }

        public string DropboxUserSecret
        {
            get
            {
                if (!_localSettings.Contains("DropboxUserSecret"))
                    return null;
                return _localSettings["DropboxUserSecret"].ToString();
            }
            set
            {
                _localSettings["DropboxUserSecret"] = value;
                _localSettings.Save();
            }
        }

        public bool ShowIntroDropboxMessage
        {
            get
            {
                if (!_localSettings.Contains("ShowIntroDropboxMessage"))
                    return true;
                return bool.Parse(_localSettings["ShowIntroDropboxMessage"].ToString());
            }
            set
            {
                _localSettings["ShowIntroDropboxMessage"] = value;
                _localSettings.Save();
            }
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
