using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.W8.Integration.Tests.Services
{
    [TestClass]
    public class PasswordAndKeyFileTests
    {

        private const string DatabasePath = "Data\\DemoDataPassAndKey.kdbx";
        private const string PasswordDatabasePassword = "UniquePassword";
        private const string KeyFileath = "Data\\DemoDataPassAndKey.key";
        [TestInitialize]
        public async Task Init()
        {


        }

        [TestMethod]
        public async Task CanOpenPasswordAndKeyFileDatabase()
        {
            var tree = await Scenarios.LoadDatabase(DatabasePath, PasswordDatabasePassword, KeyFileath);
        }
    }
}
