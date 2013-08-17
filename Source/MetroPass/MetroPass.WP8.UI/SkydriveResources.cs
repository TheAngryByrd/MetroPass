using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI
{
    public class SkydriveResources
    {
        /// <summary>
        /// The URI for the OAuth service's Authorize endpoint.
        /// </summary>
        public const string OAuthAuthorizeUri = "https://oauth.live.com/authorize";

        /// <summary>
        /// The URI for the API service endpoint.
        /// </summary>
        public const string ApiServiceUri = "https://apis.live.net/v5.0/";

        /// <summary>
        /// The applications client ID.
        /// </summary>
        public const string ClientId = ""/* insert client ID here - go to http://manage.dev.live.com to get one */ ;

        /// <summary>
        /// The applications redirect URI (does not need to exist).
        /// </summary>
        public const string RedirectUri = "https://oauth.live.com/desktop";

       
    }
}
