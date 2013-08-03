using MetroPass.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Windows.ApplicationModel;
using Windows.Storage;
using PCLStorage;

namespace MetroPass.Core.Tests.Services
{
    public class Scenarios
    {
        public async static Task<PwDatabase> LoadDatabase(string path, string password, string keyPath)
        {
            var database = await Package.Current.InstalledLocation.GetFileAsync(path);
            return await LoadDatabase(database, password, keyPath);

        }

        public async static Task<PwDatabase> LoadDatabase(IStorageFile database, string password, string keyPath)
        {
            var userKeys = new List<IUserKey>();
            var hasher = new SHA256HasherRT();
            if (!string.IsNullOrEmpty(password))
            {
                userKeys.Add(await KcpPassword.Create(password, hasher));
            }

            if (!string.IsNullOrEmpty(keyPath))
            {
                var file = await Helpers.Helpers.GetKeyFile(keyPath);
                userKeys.Add(await KcpKeyFile.Create(new WinRTFile(file), hasher));
            }


            var readerFactory = new KdbReaderFactory();

            return await readerFactory.LoadAsync(database, userKeys);
        }
    }
}
