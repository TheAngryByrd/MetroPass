using MetroPassLib.Keys;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroLib.Tests.LibTests
{
    [TestClass]
    public class PasswordAndKeyFileTests
    {
        CompositeKey composite;
        private const string DatabasePath = "Data\\DemoDataPassAndKey.kdbx";
        private const string PasswordDatabasePassword = "UniquePassword";
        private const string KeyFileath = "Data\\DemoDataPassAndKey.key";
        [TestInitialize]
        public async Task Init()
        {

            composite = new CompositeKey();

        }

        [TestMethod]
        public async Task CanOpenPasswordAndKeyFileDatabase()
        {
            composite.UserKeys.Add(await KcpPassword.Create(PasswordDatabasePassword));
            composite.UserKeys.Add(await KcpKeyFile.Create(await Helpers.GetKeyFile(KeyFileath)));
            var kdb4Tree = await Scenarios.UnlockDatabase(DatabasePath, composite);
        }

        //[TestMethod]
        //public async Task CantOpenPasswordAndKeyDatabaseWithOnlyPassword()
        //{
        //    composite.UserKeys.Add(await KcpPassword.Create(PasswordDatabasePassword));
        //    var kdb4Tree = await Scenarios.UnlockDatabase(DatabasePath, composite);
        //}

        //[TestMethod]
        //public async Task CantOpenPasswordAndKeyDatabaseWithOnlyKeyFile()
        //{
        //    composite.UserKeys.Add(await KcpKeyFile.Create(await Helpers.GetKeyFile(KeyFileath)));
        //    var kdb4Tree = await Scenarios.UnlockDatabase(DatabasePath, composite);
        //}
    }
}
