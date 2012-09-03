using Framework;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Model.Keys;
using MetroPass.Core.Services;
using MetroPass.UI.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Framework;

namespace MetroPass.UI.DataModel
{

    public sealed class PWDatabaseDataSource : BindableBase
    {
        private static PWDatabaseDataSource instance = new PWDatabaseDataSource();
        private PWDatabaseDataSource()
        {

        }
        public static PWDatabaseDataSource Instance { get { return instance; } }

        private IKdbTree _tree;
        public IKdbTree Tree
        {
            get { return _tree; }
            set
            {
                SetProperty(ref _tree, value);
            }
        }

        public void SetupDemoData()
        {
            var root = new PwGroup(null) { Name = "Root" };
            var email = new PwGroup(null) { Name = "Email" };
            var gmailAccounts = new PwGroup(null) { Name = "Gmail" };
            gmailAccounts.Entries.Add(new PwEntry(null) { Title = "Main gmail", Username = "Something@gmail.com" });
            email.SubGroups.Add(gmailAccounts);
            email.Entries.Add(new PwEntry(null) { Title = "Yahoo", Username = "Something@yahoo.com" });
            root.SubGroups.Add(email);

            var homebanking = new PwGroup(null) { Name = "Banking" };
            root.SubGroups.Add(homebanking);
            var tree = new Kdb4Tree(null);
            tree.Group = root;

            this._tree = tree;
        }

        public async Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            var factory = new KdbReaderFactory();
            this.Tree = await factory.LoadAsync(pwDatabaseFile, userKeys, percentComplete);


        }

    } 
}
