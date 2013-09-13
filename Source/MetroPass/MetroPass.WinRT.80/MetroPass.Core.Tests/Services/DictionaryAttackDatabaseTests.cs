using System.Diagnostics;
using MetroPass.WinRT.Infrastructure.Compression;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.Storage.Search;

namespace MetroPass.Core.Tests.Services
{
    [TestClass]
    public class HighEncryptionRoundDatabaseTests
    {
        CompositeKey composite;
        private const string PasswordDatabasePath = "Data\\DictionaryAttackProofDatabase.kdbx";
        private const string DatabasePassword = "UniquePassword";
        private const string KeyFileath = "Data\\DictionaryAttackProofDatabase.key";
        [TestInitialize]
        public async Task Init()
        {



        }

        [TestMethod]
        [Ignore]
        public async Task HighEncryptionRoundDatabase()
        {
            var tree = await Scenarios.LoadDatabase(PasswordDatabasePath, DatabasePassword, KeyFileath);

        }

        [TestMethod]
        public async Task KeepassFilesThatPassOrFail()
        {
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".kdbx" });
            queryOptions.FolderDepth = FolderDepth.Shallow;


            var samples = await Package.Current.InstalledLocation.GetFolderAsync("Samples");
            var query = samples.CreateFileQueryWithOptions(queryOptions);
            IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();

            var listOfFails = new List<object>();

            foreach (var file in fileList)
            {
                string lastFileToTry = file.Name;
                try
                {
                    await Scenarios.LoadDatabase(file, "password", null);

                }
                catch (Exception e)
                {
                    listOfFails.Add(lastFileToTry);
                }
            }

            Assert.AreEqual(listOfFails.Count(), 7);
        }

        [TestMethod]
        public async Task CanWriteKeePassFiles()
        {
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".kdbx" });
            queryOptions.FolderDepth = FolderDepth.Shallow;

            var samples = await Package.Current.InstalledLocation.GetFolderAsync("Samples2");
            var query = samples.CreateFileQueryWithOptions(queryOptions);
            IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();

            

            var writer = new Kdb4Writer(new Kdb4HeaderWriter(),
                      new WinRTCrypto(),
                      new MultiThreadedBouncyCastleCrypto(),
                      new SHA256HasherRT(),
                      new GZipFactoryRT());

            var listOfFails = new List<object>();

            foreach (var file in fileList)
            {
                PwDatabase database = null;
                string lastFileToTry = file.Name;

                await TryCleanup(lastFileToTry);
                try
                {

                    database = await Scenarios.LoadDatabase(file, "password", null);

                    var newFile = await KnownFolders.DocumentsLibrary.CreateFileAsync(lastFileToTry);
                    await writer.Write(database, new WinRTFile(newFile));
                    await Scenarios.LoadDatabase(newFile, "password", null);
                }
                catch (FormatException e)
                {
                    listOfFails.Add(lastFileToTry);
                }
                catch (Exception e)
                {
                    if(Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    Assert.Fail();
                }
                finally
                {
                    //TryCleanup(lastFileToTry);
                }
            }

            Assert.AreEqual(listOfFails.Count(), 7);
        }

        private async Task TryCleanup(string lastFileToTry)
        {
            try
            {
                var temp = await KnownFolders.DocumentsLibrary.GetFileAsync(lastFileToTry);
                await temp.DeleteAsync();
            }
            catch (Exception)
            {
            }
        }

    }
}
