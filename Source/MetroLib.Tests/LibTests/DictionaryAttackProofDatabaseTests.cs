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
    [Ignore]
    public class DictionaryAttackProofDatabaseTests
    {
        CompositeKey composite;
        private const string PasswordDatabasePath = "Data\\DictionaryAttackProofDatabase.kdbx";
        private const string DatabasePassword = "UniquePassword";
        private const string KeyFileath = "Data\\DictionaryAttackProofDatabase.key";
        [TestInitialize]
        public async Task Init()
        {

            composite = new CompositeKey();
      
        }

        [TestMethod]
        public async Task CanOpenDictionaryAttackProof()
        {
            composite.UserKeys.Add(await KcpPassword.Create(DatabasePassword));
            composite.UserKeys.Add(await KcpKeyFile.Create(await Helpers.GetKeyFile(KeyFileath)));
            var kdb4Tree = await Scenarios.UnlockDatabase(PasswordDatabasePath, composite);
        }

    }
}
