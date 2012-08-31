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
    public class PasswordOnlyDatabase
    {
        CompositeKey composite;
        private const string PasswordDatabasePath = "Data\\Pass.kdbx";
        private const string PasswordDatabasePassword = "UniquePassword";
        [TestInitialize]
        public async Task Init()
        {

            composite = new CompositeKey();
            composite.UserKeys.Add(await KcpPassword.Create(PasswordDatabasePassword));
        }

        [TestMethod]
        public async Task CanOpenPasswordOnlyDatabase()
        {
            var kdb4Tree = await Scenarios.UnlockDatabase(PasswordDatabasePath, composite);
        }

        
    }
}
