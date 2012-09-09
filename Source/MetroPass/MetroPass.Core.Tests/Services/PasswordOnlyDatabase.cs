using MetroPass.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Tests.Services
{
    [TestClass]
    public class PasswordOnlyDatabase
    {

        private const string PasswordDatabasePath = "Data\\Pass.kdbx";
        private const string PasswordDatabasePassword = "UniquePassword";
        [TestInitialize]
        public async Task Init()
        {


        }

        [TestMethod]
        public async Task CanOpenPasswordOnlyDatabase()
        {
            var kdb4Tree = await Scenarios.LoadDatabase(PasswordDatabasePath, PasswordDatabasePassword,null);
            var firstGroup = kdb4Tree.Group.SubGroups.First();
            Assert.AreEqual("General", firstGroup.Name);
            Assert.AreEqual(new DateTime(2012, 08, 25), firstGroup.LastAccessTime.Date);
           var firstEntry = kdb4Tree.Group.Entries.First();
           Assert.AreEqual("Notes", firstEntry.Notes);
           Assert.AreEqual("User Name", firstEntry.Username);
           Assert.AreEqual("Sample Entry", firstEntry.Title);
        }

        [TestMethod]
        public async Task CantOpenWithBadPassword()
        {
            AssertEx.ThrowsException<SecurityException>(() => Scenarios.LoadDatabase(PasswordDatabasePath, "NotPassword", null));
     
     
        }


    }
}
