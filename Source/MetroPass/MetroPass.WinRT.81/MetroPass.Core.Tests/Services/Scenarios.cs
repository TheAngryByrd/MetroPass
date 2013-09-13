using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MetroPass.WinRT.Infrastructure;
using MetroPass.WinRT.Infrastructure.Compression;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
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
                var keyfile = await Helpers.Helpers.GetKeyFile(keyPath);
                userKeys.Add(await KcpKeyFile.Create(new WinRTFile(keyfile), hasher));
            }


            var factory = new KdbReaderFactory(new WinRTCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                    new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB),
                    new SHA256HasherRT(),
                    new GZipFactoryRT());

            var file = await FileIO.ReadBufferAsync(database);
            var kdbDataReader = new MemoryStream(file.AsBytes());

           return await factory.LoadAsync(kdbDataReader, userKeys);
        }
    }
}
