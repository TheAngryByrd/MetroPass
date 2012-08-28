using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage.Streams;

namespace MetroLib.Tests
{
    public static class Helpers
    {

        public static async Task<IBuffer> GetDatabaseAsBuffer()
        {
            var database = await Package.Current.InstalledLocation.GetFileAsync("Data\\Pass.kdbx");//await KnownFolders.DocumentsLibrary.GetFileAsync("Data.kdbx");
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(database);
            return buffer;
        }
        public static async Task<IDataReader> GetDatabaseAsDatareaderAsync()
        {
            var buffer = await GetDatabaseAsBuffer();
            IDataReader reader = DataReader.FromBuffer(buffer);
            //reader.ByteOrder = ByteOrder.LittleEndian;
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            return reader;
        }
    }
}
