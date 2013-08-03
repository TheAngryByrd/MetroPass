using MetroPass.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
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
            var factory = new KdbReaderFactory();
            this.PwDatabase = await factory.LoadAsync(pwDatabaseFile, userKeys, percentComplete);
        }

        public async Task CreatePwDatabase(IStorageFile pwDatabaseFil )
        {

        }

        public async Task SavePwDatabase()
        {
            var factory = new KdbWriterFactory();

            var writer = factory.CreateWriter(PwDatabase.Tree);

            await writer.Write(PwDatabase, new WinRTFile(StorageFile));

        }
    } 
}