using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI.DataModel
{
    public interface IDatabaseInfoRepository
    {
        Task DeleteKeyFile(DatabaseInfo databaseInfo);

        Task<IEnumerable<DatabaseInfo>> GetDatabaseInfo();

        Task<DatabaseInfo> GetDatabaseInfo(string databaseName);

        Task SaveDatabaseFromDatasouce(string databaseName, string cloudprovider, string path, Stream database);

        Task SaveKeyFileFromDatasouce(string databaseName,string keyFileName, Stream keyFile);
    }
}