using System.IO;
using Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetroPass.WinRT.Infrastructure.Compression;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using PCLStorage;
using Windows.Storage;

namespace MetroPass.UI.DataModel
{

    public sealed class PWDatabaseDataSource
    {
        private static PWDatabaseDataSource instance = new PWDatabaseDataSource();
        private PWDatabaseDataSource()
        {

        }
        public static PWDatabaseDataSource Instance { get { return instance; } }

        public IStorageFile StorageFile;
        private PwDatabase _pwDatabase;
        public PwDatabase PwDatabase
        {
            get { return _pwDatabase; }
            set { _pwDatabase = value; }
        }

        public void SetupDemoData()
        {
            //var root = new PwGroup(null) { Name = "Root" };
            //var email = new PwGroup(null) { Name = "Email" };
            //var gmailAccounts = new PwGroup(null) { Name = "Gmail" };
            //gmailAccounts.AddEntry(new PwEntry(null) { Title = "Main gmail", Username = "Something@gmail.com" });
            //email.AddSubGroup(gmailAccounts);
            //email.AddEntry(new PwEntry(null) { Title = "Yahoo", Username = "Something@yahoo.com" });
            //root.AddSubGroup(email);

            //var homebanking = new PwGroup(null) { Name = "Banking" };
            //root.AddSubGroup(homebanking);
            //var tree = new Kdb4Tree(null);
            //tree.Group = root;
            //var pwDatabase = new PwDatabase(null);
            //pwDatabase.Tree = tree;
            //this.PwDatabase = pwDatabase;
        }

        public async Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            StorageFile = pwDatabaseFile;
            var factory = new KdbReaderFactory(new WinRTCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                    new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB),
                    new SHA256HasherRT(),
                    new GZipFactoryRT());
           
            var file = await FileIO.ReadBufferAsync(pwDatabaseFile);           
            MemoryStream kdbDataReader = new MemoryStream(file.AsBytes());

            this.PwDatabase = await factory.LoadAsync(kdbDataReader, userKeys, percentComplete);
        }

        public async Task CreatePwDatabase(IStorageFile pwDatabaseFil )
        {

        }

        public async Task SavePwDatabase()
        {
            var factory = new KdbWriterFactory(new WinRTCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                    new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB),
                    new SHA256HasherRT(),
                    new GZipFactoryRT());

            var writer = factory.CreateWriter(PwDatabase.Tree);

            await writer.Write(PwDatabase, new WinRTFile(StorageFile));

        }
    } 
}