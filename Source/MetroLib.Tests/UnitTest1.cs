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

namespace MetroLibTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var database = await KnownFolders.DocumentsLibrary.GetFileAsync("Data.kdbx");
            DataReader reader = new DataReader(await database.OpenSequentialReadAsync());
            
            var buffer =await  Windows.Storage.FileIO.ReadBufferAsync(database);
            var x = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = x.CreateHash();
            hash.Append(buffer);
            var hashedBuffer = hash.GetValueAndReset();
            
        }

        [TestMethod]
        public async Task ShouldReadHeaders()
        {
            
            var database = await KnownFolders.DocumentsLibrary.GetFileAsync("Data.kdbx");
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(database);
            IDataReader reader =  DataReader.FromBuffer(buffer);
            reader.ByteOrder = ByteOrder.LittleEndian;
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            reader.ReadBytes(new byte[12]);
           
            Kdb4File kdb = new Kdb4File();
            while (true)
            {
                if (kdb.ReadHeaderField(reader) == false) { break; }
            }
        }
        
    }
}
