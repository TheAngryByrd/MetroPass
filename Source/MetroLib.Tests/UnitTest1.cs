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
using MetroPassLib.Security;
using MetroPassLib.Keys;

namespace MetroLibTests
{
   

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {

            var database = await Package.Current.InstalledLocation.GetFileAsync("Data\\Pass.kdbx");
            
            DataReader reader = new DataReader(await database.OpenSequentialReadAsync());
            
            var buffer =await  Windows.Storage.FileIO.ReadBufferAsync(database);
            var x = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = x.CreateHash();
            hash.Append(buffer);
            var hashedBuffer = hash.GetValueAndReset();
            
            
            
        }

        [TestMethod]
        public async Task ShouldTransformKey()
        {
            var kdb = new Kdb4File(new PwDatabase());
            var database = await GetDatabaseAsDatareaderAsync();
            kdb.ReadHeader(database);

            var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword("UniquePassword"));
            var rawCompositeKey = await composite.CreateRawCompositeKey32();
            var rawCompositeKeyBytes = rawCompositeKey.AsBytes();
            SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var transformSeedKey = symKeyProvider.CreateSymmetricKey( kdb.pbTransformSeed.AsBuffer());


            IBuffer iv = null;

            var decryptedKey = await CompositeKey.TransformKeyManagedAsync(rawCompositeKey, transformSeedKey, iv, 6000);
            var actual = decryptedKey.AsBytes();
        }


        [TestMethod]
        public async Task ShouldCreateAESKey()
        {
             var kdb = new Kdb4File(new PwDatabase());
             var database = await GetDatabaseAsDatareaderAsync();
             kdb.ReadHeader(database);
             MemoryStream ms = new MemoryStream();
             ms.Write(kdb.pbMasterSeed, 0, 32);
        }

        [TestMethod]
        public async Task ShouldGenerate32BitKeyFromCompositeKey()
        {
            var pwDataBase = new PwDatabase();
            var kdb = new Kdb4File(pwDataBase);

            var database = await GetDatabaseAsDatareaderAsync();
            kdb.ReadHeader(database);
         
        }

        [TestMethod]
        public async Task ShouldReadHeaders()
        {

            IDataReader reader = await GetDatabaseAsDatareaderAsync();
            reader.ReadBytes(new byte[12]);
           
            Kdb4File kdb = new Kdb4File(new PwDatabase());
            while (true)
            {
                if (kdb.ReadHeaderField(reader) == false) { break; }
            }
        }

        private static async Task<IDataReader> GetDatabaseAsDatareaderAsync()
        {
            var database = await Package.Current.InstalledLocation.GetFileAsync("Data\\Pass.kdbx");//await KnownFolders.DocumentsLibrary.GetFileAsync("Data.kdbx");
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(database);
            IDataReader reader = DataReader.FromBuffer(buffer);
            reader.ByteOrder = ByteOrder.LittleEndian;
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            return reader;
        }
        
    }
}
