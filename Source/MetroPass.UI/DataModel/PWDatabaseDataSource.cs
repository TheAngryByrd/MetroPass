using MetroPass.UI.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.UI.Helpers;
using Windows.Storage;
using MetroPassLib.Keys;
using MetroPassLib;
using Windows.ApplicationModel;
using Windows.Storage.Streams;


namespace MetroPass.UI.DataModel
{
    public sealed class PWDatabaseDataSource
    {


        public static EntryGroup Root
        {
            get;
            set;
        }

        public static void SetupDemoData()
        {
            var root = new EntryGroup() { Name = "Root" };
            var email = new EntryGroup() { Name = "Email" };
            var gmailAccounts = new EntryGroup() { Name = "Gmail" };
            gmailAccounts.AddEntry(new Entry() { Title = "Main gmail", Username = "Something@gmail.com" });
            email.EntryGroups.Add(gmailAccounts);
            email.AddEntry(new Entry() { Title = "Yahoo", Username = "Something@yahoo.com" });
            root.AddEntryGroup(email);

            var homebanking = new EntryGroup() { Name = "Banking" };
            root.AddEntryGroup(homebanking);


            Root = root;
        }

        public static async Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            var pwDatabase = new PwDatabase();
            pwDatabase.MasterKey = new CompositeKey() { UserKeys = userKeys };
            var kdb4 = new Kdb4File(pwDatabase);
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(pwDatabaseFile);
            IDataReader pwDatabaseDatareader = DataReader.FromBuffer(buffer);
            pwDatabaseDatareader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

            var treeView =  await kdb4.Load(pwDatabaseDatareader, Kdb4Format.Default,percentComplete);
            Root = ParseGroup(treeView.Group);
            
        }

        private static EntryGroup ParseGroup(PwGroup group)
        {
            EntryGroup eGroup = new EntryGroup();
            eGroup.Name = group.Name;


            foreach (var element in group.Entries)
            {
                eGroup.AddEntry(ParseEntry(element));
            }

            foreach (var element in group.SubGroups)
            {
                eGroup.AddEntryGroup(ParseGroup(element));
            }

            return eGroup;

          
        }

        private static Entry ParseEntry(PwEntry element)
        {
            return new Entry { Title = element.Title, Username = element.Username, Password = element.Password };
        }
    }
    public abstract class EntryDataCommon : BindableBase
    {
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime ExpireTime { get; set; }

        public EntryGroup Parent { get; set; }
    }

    public class EntryGroup : EntryDataCommon
    {
        public string Name { get; set; }

        public ObservableCollection<EntryGroup> EntryGroups { get; set; }
        public ObservableCollection<EntryGroup> EntryGroupsWithEntries
        {
            get
            {
                var temp = new ObservableCollection<EntryGroup>();

                temp.AddRange(EntryGroups);
                temp.Add(new EntryGroup() { Name = "Entries", Entries = this.Entries });

                return temp;
            }
        }
        public ObservableCollection<Entry> Entries { get; set; }
        public ObservableCollection<object> AllTogetherNow
        {
            get
            {
                var temp = new ObservableCollection<object>();

                temp.AddRange(EntryGroups);
                temp.AddRange(Entries);

                return temp;
            }
        }
        public EntryGroup()
        {
            EntryGroups = new ObservableCollection<EntryGroup>();
            Entries = new ObservableCollection<Entry>();
        }

        public void AddEntryGroup(EntryGroup entryGroup)
        {
            this.EntryGroups.Add(entryGroup);
            entryGroup.Parent = this;
        }
        public void AddEntry(Entry entry)
        {
            this.Entries.Add(entry);
            entry.Parent = this;
        }

    }
    public class Entry : EntryDataCommon
    {
        public string Title { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


    }
}
