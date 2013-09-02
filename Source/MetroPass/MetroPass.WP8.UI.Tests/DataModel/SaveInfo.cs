using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MetroPass.WP8.UI.DataModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MetroPass.WP8.UI.Tests.DataModel
{
    [TestClass]
    public class DatabaseRepositoryTests
    {

        private StorageFolder _installedPath;
        private DatabaseInfoRepository _databaseInfoRepository;

        [TestInitialize]
        public async Task Init()
        {
            var path =  Package.Current.InstalledLocation;
            _installedPath = await path.CreateFolderAsync("Databases", CreationCollisionOption.OpenIfExists);
        
            _databaseInfoRepository = new DatabaseInfoRepository();
        }

        [TestMethod]
        public async Task SaveInfo()
        {
            var info = new Info(new XDocument());
            info.DatabaseCloudPath = "CloudPath";
            info.DatabaseCloudProvider = "CloudProvider";
            info.DatabasePath = "DatabasePath";

            var databaseInfoRepository = new DatabaseInfoRepository();

            await databaseInfoRepository.SaveInfo(_installedPath, info);

            var infoFromFileSystem = await _databaseInfoRepository.GetInfo(_installedPath);
        
            Assert.AreEqual(info.DatabaseCloudPath, infoFromFileSystem.DatabaseCloudPath);
            Assert.AreEqual(info.DatabaseCloudProvider, infoFromFileSystem.DatabaseCloudProvider);
            Assert.AreEqual(info.DatabasePath, infoFromFileSystem.DatabasePath);
        }


        [TestMethod]
        public async Task GetInfoDoesntExist()
        {
            var info = await _databaseInfoRepository.GetInfo(_installedPath);

            Assert.AreEqual("", info.DatabasePath);
            Assert.AreEqual("", info.DatabaseCloudPath);
            Assert.AreEqual("", info.DatabaseCloudProvider);
            Assert.AreEqual("", info.KeyFilePath);
        }

        [TestMethod]
        public async Task SaveDatabaseFromDatasouce()
        {
            var bytes = new byte[100];
            new Random().NextBytes(bytes);
            var memoryStream = new MemoryStream(bytes);
            await _databaseInfoRepository.SaveDatabaseFromDatasouce("Name", "Provider", "CloudPath", memoryStream);

            var databases = await _databaseInfoRepository.GetDatabaseInfo();

            Assert.AreEqual(1, databases.Count());
        }

        [TestCleanup]
        public async Task Cleanup()
        {
           await _installedPath.DeleteAsync();
        }
    }
}
