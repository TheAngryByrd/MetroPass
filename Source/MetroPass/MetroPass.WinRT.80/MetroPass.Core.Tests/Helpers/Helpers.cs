using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.Core.W8.Tests.Helpers
{
    public static class Helpers
    {

        public static async Task<IDataReader> GetDatabaseAsDatareaderAsync(string path)
        {
            var database = await Package.Current.InstalledLocation.GetFileAsync(path);
            var buffer = await FileIO.ReadBufferAsync(database);
            IDataReader reader = DataReader.FromBuffer(buffer);
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;
            return reader;
        }


        public static async Task<IStorageFile> GetKeyFile(string path)
        {
            return await Package.Current.InstalledLocation.GetFileAsync(path);
        }

    }

    public static class AssertEx
    {
        public async static void ThrowsException<T>(Func<Task> task)
        {
            bool exceptionRaised = false;

            try
            {
                 task().Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.Flatten().InnerException is T)
                    exceptionRaised = true;
            }

            if (!exceptionRaised)
                Assert.Fail("Did not receive exception {0}", typeof(T).Name);
        }
      
    }
}
