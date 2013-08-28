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

        public LiveConnectSession SkydriveSession { get; set; }
    }
}
