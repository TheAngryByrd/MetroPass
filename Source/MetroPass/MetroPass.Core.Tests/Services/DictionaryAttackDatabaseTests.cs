using Metropass.Core.PCL.Model.Kdb4.Keys;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
 
    }
}
