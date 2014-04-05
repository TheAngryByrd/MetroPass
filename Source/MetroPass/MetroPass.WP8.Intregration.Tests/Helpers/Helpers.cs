using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.WP8.Intregration.Tests.Helpers
{
    public static class Helpers
    {

        public static async Task<IDataReader> GetDatabaseAsDatareaderAsync(string path)
        {
            var database = await Package.Current.InstalledLocation.GetFileAsync(path);
            var buffer = await database.AsBuffer();
            IDataReader reader = DataReader.FromBuffer(buffer);
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;
            return reader;
        }


        public static async Task<IStorageFile> GetKeyFile(string path)
        {
            return await Package.Current.InstalledLocation.GetFileAsync(path);
        }

        public static async Task<IBuffer> AsBuffer(this IStorageFile file)
        {
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes.AsBuffer();
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
