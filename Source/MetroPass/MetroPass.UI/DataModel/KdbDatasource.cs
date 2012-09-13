﻿using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Model.Keys;
using MetroPass.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private PwDatabase _pwDatabase;
        public PwDatabase PwDatabase
        {
            get { return _pwDatabase; }
            set { _pwDatabase = value; }
        }

        public void SetupDemoData()
        {
            var root = new PwGroup(null) { Name = "Root" };
            var email = new PwGroup(null) { Name = "Email" };
            var gmailAccounts = new PwGroup(null) { Name = "Gmail" };
            gmailAccounts.AddEntry(new PwEntry(null) { Title = "Main gmail", Username = "Something@gmail.com" });
            email.AddSubGroup(gmailAccounts);
            email.AddEntry(new PwEntry(null) { Title = "Yahoo", Username = "Something@yahoo.com" });
            root.AddSubGroup(email);

            var homebanking = new PwGroup(null) { Name = "Banking" };
            root.AddSubGroup(homebanking);
            var tree = new Kdb4Tree(null);
            tree.Group = root;
            var pwDatabase = new PwDatabase(null);
            pwDatabase.Tree = tree;
            this.PwDatabase = pwDatabase;
        }

        public async Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            var factory = new KdbReaderFactory();
            this.PwDatabase = await factory.LoadAsync(pwDatabaseFile, userKeys, percentComplete);
        }
    } 
}