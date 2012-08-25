using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Security;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using MetroPassLib;
using Windows.ApplicationModel;
using System.IO;

namespace MetroLibTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            
            var database = await Package.Current.InstalledLocation.GetFileAsync("Data\\DemoData.kdbx");//await KnownFolders.DocumentsLibrary.GetFileAsync("Data.kdbx");
            
            DataReader reader = new DataReader(await database.OpenSequentialReadAsync());
            
            var buffer =await  Windows.Storage.FileIO.ReadBufferAsync(database);
            var x = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = x.CreateHash();
            hash.Append(buffer);
            var hashedBuffer = hash.GetValueAndReset();
            
        }

        [TestMethod]
        public async Task ShouldCreateAESKey()
        {
             var kdb = new Kdb4File(new PwDatabase());
             var database = await GetDatabaseAsDatareader();
             kdb.ReadHeader(database);
             MemoryStream ms = new MemoryStream();
             ms.Write(kdb.pbMasterSeed, 0, 32);
        }

        [TestMethod]
        public async Task ShouldGenerate32BitKeyFromCompositeKey()
        {
            var pwDataBase = new PwDatabase();
            var kdb = new Kdb4File(pwDataBase);

            var database = await GetDatabaseAsDatareader();
            kdb.ReadHeader(database);
         
        }

        [TestMethod]
        public async Task ShouldReadHeaders()
        {

            IDataReader reader = await GetDatabaseAsDatareader();
            reader.ReadBytes(new byte[12]);
           
            Kdb4File kdb = new Kdb4File(new PwDatabase());
            while (true)
            {
                if (kdb.ReadHeaderField(reader) == false) { break; }
            }
        }

        private static async Task<IDataReader> GetDatabaseAsDatareader()
        {
            var database = await Package.Current.InstalledLocation.GetFileAsync("Data\\DemoData.kdbx");//await KnownFolders.DocumentsLibrary.GetFileAsync("Data.kdbx");
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(database);
            IDataReader reader = DataReader.FromBuffer(buffer);
            reader.ByteOrder = ByteOrder.LittleEndian;
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            return reader;
        }
        
    }
}
