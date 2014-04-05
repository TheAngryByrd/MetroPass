using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using MetroPass.WP8.Infrastructure.Compression;
using MetroPass.WP8.Infrastructure.Cryptography;
using MetroPass.WP8.Infrastructure.Hashing;
using MetroPass.WP8.Intregration.Tests.Helpers;
using PCLStorage;

namespace MetroPass.WP8.Intregration.Tests.Services
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
            var hasher = new SHA256HasherWP8();
            if (!string.IsNullOrEmpty(password))
            {
                userKeys.Add(await KcpPassword.Create(password, hasher));
            }

            if (!string.IsNullOrEmpty(keyPath))
            {
                var keyfile = await Helpers.Helpers.GetKeyFile(keyPath);
                userKeys.Add(await KcpKeyFile.Create(new WP8File(keyfile), hasher));
            }


            var factory = new KdbReaderFactory(new ManagedCrypto(),
                    new MultiThreadedBouncyCastleCrypto(),
                    new SHA256HasherWP8(),
                    new GZipFactoryWP8());
            var file = await database.AsBuffer();
            var kdbDataReader = new MemoryStream(file.ToArray());

           return await factory.LoadAsync(kdbDataReader, userKeys);
        }
    }
}
