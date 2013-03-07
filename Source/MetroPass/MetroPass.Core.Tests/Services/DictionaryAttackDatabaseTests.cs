using MetroPass.Core.Model.Keys;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.Core.Tests.Helpers;
using Windows.Storage;
using MetroPass.Core.Services;
using Windows.ApplicationModel;
using MetroPass.Core.Interfaces;

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
    }
}
