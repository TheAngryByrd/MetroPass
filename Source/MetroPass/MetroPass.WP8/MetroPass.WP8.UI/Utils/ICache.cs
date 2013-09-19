using System;
using System.IO.IsolatedStorage;
using Microsoft.Live;

namespace MetroPass.WP8.UI.Utils
{
    public interface ICache
    {
        bool ShowIntroDropboxMessage { get; set; }

        string DatabaseName { get; set; }

        LiveConnectSession SkydriveSession { get; set; }

        string DropboxUserToken { get; set; }
        string DropboxUserSecret { get; set; }
        DownloadFileNavigationCache DownloadFileNavigationCache { get; set; }
    }
}