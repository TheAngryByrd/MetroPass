using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Model.Keys;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MetroPass.Core.Tests.ComparedToMainKeepass
{
    //[TestClass]
    //public class PassingTests
    //{
    //    IDataReader reader;
    //    Kdb4File kdb;
    //    PwDatabase database;
    //    CompositeKey composite;
    //    [TestInitialize]
    //    public async Task Init()
    //    {
    //        database = new PwDatabase();
    //        reader = await Helpers.GetDatabaseAsDatareaderAsync("Data\\Pass.kdbx");
    //        kdb = new Kdb4File(database);
    //        composite = new CompositeKey();
    //        composite.UserKeys.Add(await KcpPassword.Create("UniquePassword"));
    //    }

    //    [TestCleanup]
    //    public async Task Cleanup()
    //    {

    //    }

    //    [TestMethod]
    //    public async Task CanReadHeader()
    //    {

    //        kdb.ReadHeader(reader);


    //        var a = "9/BEU5hRkI54EZcmkWxYPg==";
    //        CollectionAssert.AreEqual(Convert.FromBase64String(a), kdb.pbEncryptionIV.AsBytes());

    //        var a1 = "CXq3PMhnBOrOrkXmr2MKEAUiIIioNKPChN186EBwq/s=";
    //        CollectionAssert.AreEqual(Convert.FromBase64String(a1), kdb.pbMasterSeed.AsBytes());

    //        var a2 = "6tDlwZfwES4jAQzLisWdpNdnuTYyDZfflEdbshzdgi8=";
    //        CollectionAssert.AreEqual(Convert.FromBase64String(a2), kdb.pbProtectedStreamKey.AsBytes());

    //        var a3 = "EIEekEuQQa412y27eEKayl+4F5J5T/AzysBpZSlrQgE=";
    //        CollectionAssert.AreEqual(Convert.FromBase64String(a3), kdb.pbStreamStartBytes.AsBytes());

    //        var a4 = "4Lx6nKqWNZNqOrq+mKNBSFaam/eLHPWKg6bfMYXidGg=";
    //        CollectionAssert.AreEqual(Convert.FromBase64String(a4), kdb.pbTransformSeed.AsBytes());

    //    }

    //    [TestMethod]
    //    public async Task CanCreateRawCompositeKey32()
    //    {

    //        var key = await composite.CreateRawCompositeKey32();

    //        Assert.AreEqual("8qJAUrflDYhubJ/yBlHYsf7Pqh9PDL52MPqsm6ARtFw=", Convert.ToBase64String(key.AsBytes()));
    //        CollectionAssert.AreEqual(key.AsBytes(), Convert.FromBase64String("8qJAUrflDYhubJ/yBlHYsf7Pqh9PDL52MPqsm6ARtFw="));
    //    }

    //    [TestMethod]
    //    public async Task CanTransformKeyManaged()
    //    {


    //        var key = await composite.CreateRawCompositeKey32();

    //        kdb.ReadHeader(reader);

    //        SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
    //        var transformSeedKey = symKeyProvider.CreateSymmetricKey(kdb.pbTransformSeed);

    //        var newKey = await composite.TransformKeyManagedAsync(key, transformSeedKey, null, kdb.pwDatabase.KeyEncryptionRounds);

    //        Assert.AreEqual("+BK6cYYfLN+c6dLZU3871jgd33phMPk59ChDbY+a3M0=", Convert.ToBase64String(newKey.AsBytes()));
    //    }

    //    [TestMethod]
    //    public async Task CanTransformKey()
    //    {


    //        var key = await composite.CreateRawCompositeKey32();
    //        kdb.ReadHeader(reader);
    //        SymmetricKeyAlgorithmProvider symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
    //        var transformSeedKey = symKeyProvider.CreateSymmetricKey(kdb.pbTransformSeed);


    //        var transformedKey = await composite.TransformKeyAsync(key, transformSeedKey, kdb.pwDatabase.KeyEncryptionRounds);
    //        Assert.AreEqual("PVQ3x9u0TvpGa0d/G65UBkCDn1mnS+XxjPz3gPNj1J8=", Convert.ToBase64String(transformedKey.AsBytes()));
    //    }

    //    [TestMethod]
    //    public async Task CanGenerateKey()
    //    {


    //        kdb.ReadHeader(reader);

    //        var key = await composite.GenerateKeyAsync(kdb.pbTransformSeed, kdb.pwDatabase.KeyEncryptionRounds);

    //        Assert.AreEqual("PVQ3x9u0TvpGa0d/G65UBkCDn1mnS+XxjPz3gPNj1J8=", Convert.ToBase64String(key.AsBytes()));
    //    }

    //    [TestMethod]
    //    public async Task CanGenerateAesKey()
    //    {

    //        database.MasterKey = composite;
    //        kdb.ReadHeader(reader);

    //        var aesKey = await kdb.GenerateAESKey();

    //        Assert.AreEqual("6ZC0KkygMGm19gMMqOQLGihZ47kXf+Iy0ucrM6UTyEY=", Convert.ToBase64String(aesKey.AsBytes()));
    //    }

    //    [TestMethod]
    //    public async Task CanDecryptDatabase()
    //    {

    //        database.MasterKey = composite;
    //        kdb.ReadHeader(reader);

    //        var aesKey = await kdb.GenerateAESKey();


    //        var decrypedDatabase = kdb.DecryptDatabase(reader.DetachBuffer(), aesKey);
    //    }

    //    [TestMethod]
    //    public async Task CanDecompressDatabase()
    //    {

    //        database.MasterKey = composite;
    //        kdb.ReadHeader(reader);

    //        var aesKey = await kdb.GenerateAESKey();


    //        var decrypedDatabase = kdb.DecryptDatabase(reader.DetachBuffer(), aesKey);

    //        var decompressed = kdb.ConfigureStream(decrypedDatabase);
    //    }

    //    [TestMethod]
    //    public async Task CanGenerateCryptoStream()
    //    {

    //        database.MasterKey = composite;
    //        kdb.ReadHeader(reader);

    //        var crypto = kdb.GenerateCryptoRandomStream();
    //    }

    //    [TestMethod]
    //    public async Task CanCreateXmlDocumentFromDecompressedDatabase()
    //    {

    //        database.MasterKey = composite;
    //        kdb.ReadHeader(reader);

    //        var aesKey = await kdb.GenerateAESKey();


    //        var decrypedDatabase = kdb.DecryptDatabase(reader.DetachBuffer(), aesKey);

    //        var decompressed = kdb.ConfigureStream(decrypedDatabase);

    //        var xml = Kdb4Parser.CreateXmlReader(decompressed);

    //        Assert.IsNotNull(xml);
    //    }

    //    [TestMethod]
    //    public async Task CanParseTree()
    //    {

    //        database.MasterKey = composite;
    //        kdb.ReadHeader(reader);

    //        var aesKey = await kdb.GenerateAESKey();


    //        var decrypedDatabase = kdb.DecryptDatabase(reader.DetachBuffer(), aesKey);

    //        var decompressed = kdb.ConfigureStream(decrypedDatabase);

    //        var crypto = kdb.GenerateCryptoRandomStream();

    //        var kdb4Parser = new Kdb4Parser(crypto);

    //        var tree = kdb4Parser.Parse(decompressed);
    //    }
    //}
}
