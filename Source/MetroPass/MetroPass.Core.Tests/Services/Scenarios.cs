using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Model.Keys;
using MetroPass.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

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
            if (!string.IsNullOrEmpty(password))
            {
                userKeys.Add(await KcpPassword.Create(password, new SHA256HasherRT()));
            }

            if (!string.IsNullOrEmpty(keyPath))
            {
                var file = await Helpers.Helpers.GetKeyFile(keyPath);
                userKeys.Add(await KcpKeyFile.Create(file));
            }


            var readerFactory = new KdbReaderFactory();

            return await readerFactory.LoadAsync(database, userKeys);
        }
    }
}
