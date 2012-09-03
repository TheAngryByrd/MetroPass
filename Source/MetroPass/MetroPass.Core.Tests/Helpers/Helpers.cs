using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MetroPass.Core.Tests.Helpers
{
    public static class Helpers
    {

        public static async Task<IDataReader> GetDatabaseAsDatareaderAsync(string path)
        {
            var database = await Package.Current.InstalledLocation.GetFileAsync(path);
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(database);
            IDataReader reader = DataReader.FromBuffer(buffer);
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            return reader;
        }


        public static async Task<IStorageFile> GetKeyFile(string path)
        {
            return await Package.Current.InstalledLocation.GetFileAsync(path);
        }

    }
}
