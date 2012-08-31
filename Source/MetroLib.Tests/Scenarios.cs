using MetroPassLib;
using MetroPassLib.Kdb4;
using MetroPassLib.Keys;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MetroLib.Tests
{
    [TestClass]
    public class Scenarios
    {
        [TestMethod]
        public static async Task<Kdb4Tree> UnlockDatabase(string databasePath, CompositeKey compositeKey)
        {
            var pwDatabase = new PwDatabase();
            pwDatabase.MasterKey = compositeKey;
            var kdb4 = new Kdb4File(pwDatabase);
            IDataReader databaseDatareader = await Helpers.GetDatabaseAsDatareaderAsync(databasePath);
            return await kdb4.Load(databaseDatareader, Kdb4Format.Default);

        }
    }
}
