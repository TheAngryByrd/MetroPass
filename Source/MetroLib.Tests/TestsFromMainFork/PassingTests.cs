using MetroPassLib;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using MetroPassLib.Helpers;
using MetroPassLib.Keys;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Diagnostics;
using System.IO;

namespace MetroLib.Tests.TestsFromMainFork
{
    [TestClass]
    public class PassingTests
    {
        IDataReader reader;
        Kdb4File kdb;
        PwDatabase database;
        [TestInitialize]
        public async Task Init()
        {
             database = new PwDatabase();
            reader = await Helpers.GetDatabaseAsDatareaderAsync();
            kdb = new Kdb4File(database);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
   
        }

        [TestMethod]
        public async Task CanReadHeader()
        {

            kdb.ReadHeader(reader);


            var a = "9/BEU5hRkI54EZcmkWxYPg==";
            CollectionAssert.AreEqual(Convert.FromBase64String(a), kdb.pbEncryptionIV.AsBytes());

            var a1 = "CXq3PMhnBOrOrkXmr2MKEAUiIIioNKPChN186EBwq/s=";
            CollectionAssert.AreEqual(Convert.FromBase64String(a1), kdb.pbMasterSeed.AsBytes());

            var a2 = "6tDlwZfwES4jAQzLisWdpNdnuTYyDZfflEdbshzdgi8=";
            CollectionAssert.AreEqual(Convert.FromBase64String(a2), kdb.pbProtectedStreamKey.AsBytes());

            var a3 = "EIEekEuQQa412y27eEKayl+4F5J5T/AzysBpZSlrQgE=";
            CollectionAssert.AreEqual(Convert.FromBase64String(a3), kdb.pbStreamStartBytes.AsBytes());

            var a4 = "4Lx6nKqWNZNqOrq+mKNBSFaam/eLHPWKg6bfMYXidGg=";
            CollectionAssert.AreEqual(Convert.FromBase64String(a4), kdb.pbTransformSeed.AsBytes());

        }

        [TestMethod]
        public async Task CanCreateRawCompositeKey32()
        {
            var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword("UniquePassword"));

            var key = await composite.CreateRawCompositeKey32();

            Assert.AreEqual("8qJAUrflDYhubJ/yBlHYsf7Pqh9PDL52MPqsm6ARtFw=", Convert.ToBase64String(key.AsBytes()));
            CollectionAssert.AreEqual(key.AsBytes(), Convert.FromBase64String("8qJAUrflDYhubJ/yBlHYsf7Pqh9PDL52MPqsm6ARtFw="));
        }

        [TestMethod]
        public async Task CanTransformKeyManaged()
        {
  
            var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword("UniquePassword"));

            var key = await composite.CreateRawCompositeKey32();

            kdb.ReadHeader(reader);

            SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var transformSeedKey = symKeyProvider.CreateSymmetricKey(kdb.pbTransformSeed);
            
            var newKey =  await composite.TransformKeyManagedAsync(key, transformSeedKey, null, kdb.pwDatabase.KeyEncryptionRounds);

            Assert.AreEqual("+BK6cYYfLN+c6dLZU3871jgd33phMPk59ChDbY+a3M0=", Convert.ToBase64String(newKey.AsBytes()));
        }

        [TestMethod]
        public async Task CanTransformKey()
        {
            var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword("UniquePassword"));

            var key = await composite.CreateRawCompositeKey32();
            kdb.ReadHeader(reader);
            SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var transformSeedKey = symKeyProvider.CreateSymmetricKey(kdb.pbTransformSeed);


            var transformedKey = await composite.TransformKeyAsync(key, transformSeedKey, kdb.pwDatabase.KeyEncryptionRounds);
            Assert.AreEqual("PVQ3x9u0TvpGa0d/G65UBkCDn1mnS+XxjPz3gPNj1J8=", Convert.ToBase64String(transformedKey.AsBytes()));
        }

        [TestMethod]
        public async Task CanGenerateKey()
        {
            var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword("UniquePassword"));

            kdb.ReadHeader(reader);

            var key = await composite.GenerateKeyAsync(kdb.pbTransformSeed, kdb.pwDatabase.KeyEncryptionRounds);

            Assert.AreEqual("PVQ3x9u0TvpGa0d/G65UBkCDn1mnS+XxjPz3gPNj1J8=", Convert.ToBase64String(key.AsBytes()));
        }

        [TestMethod]
        public async Task CanGenerateAesKey()
        {
            var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword("UniquePassword"));
            database.MasterKey = composite;
            kdb.ReadHeader(reader);

            var aesKey = await kdb.GenerateAESKey();

            Assert.AreEqual("6ZC0KkygMGm19gMMqOQLGihZ47kXf+Iy0ucrM6UTyEY=", Convert.ToBase64String(aesKey.AsBytes()));
        }

        [TestMethod]
        public async Task CanDecryptDatabase()
        {
            var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword("UniquePassword"));
            database.MasterKey = composite;
            kdb.ReadHeader(reader);

            var aesKey = await kdb.GenerateAESKey();
            

            var decrypedDatabase = kdb.DecryptDatabase(reader.DetachBuffer(), aesKey);
        }
    }
}
