using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MetroPass.WP8.UI.DataModel
{
    public interface IDatabaseInfoRepository
    {
        Task<IEnumerable<DatabaseInfo>> GetDatabaseInfo();

        Task SaveDatabaseFromDatasouce(string databaseName, string cloudprovider, string path, Stream database);

        Task SaveKeyFileFromDatasouce(string databaseName,string keyFileName, Stream keyFile);
    }
}