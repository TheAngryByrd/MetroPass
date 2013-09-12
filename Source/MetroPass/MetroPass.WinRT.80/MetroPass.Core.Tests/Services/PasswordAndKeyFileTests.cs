using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Tests.Services
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
