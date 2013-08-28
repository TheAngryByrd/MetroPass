using MetroPass.WP8.UI.DataModel;
using Microsoft.Live;

namespace MetroPass.WP8.UI.Utils
{
    public class Cache
    {
        private static Cache instance = new Cache();
        public DatabaseInfo DatabaseInfo { get; set; }

        private Cache()
        {
        }

        public static Cache Instance { get { return instance; } }

        public LiveConnectSession SkydriveSession { get; set; }
    }
}
