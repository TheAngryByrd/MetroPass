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
using Windows.ApplicationModel;
using Windows.Storage.Streams;

namespace MetroPass.Core.Tests.Services
{
    public class Scenarios
    {
        public async static Task<IKdbTree> LoadDatabase(string path, string password, string keyPath)
        {
            var userKeys = new List<IUserKey>();
            if (!string.IsNullOrEmpty(password))
            {
                userKeys.Add(await KcpPassword.Create(password));
            }

            if (!string.IsNullOrEmpty(keyPath))
            {
                var file = await Helpers.Helpers.GetKeyFile(keyPath);
                userKeys.Add(await KcpKeyFile.Create(file));
            }
       

            var readerFactory = new KdbReaderFactory();
            var database = await Package.Current.InstalledLocation.GetFileAsync(path);
            IKdbTree tree = (await readerFactory.LoadAsync(database, userKeys)).Tree;
            return tree;
        }
    }
}
