using MetroPass.UI.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.UI.Helpers;

namespace MetroPass.UI.ViewModels
{
    public class GroupListPageViewModel :BindableBase
    {

        EntryGroup _root = new EntryGroup();
        public EntryGroup Root 
        {
            get
            {

                return _root;
            }
            set
            {
                SetProperty(ref _root, value);
            }
        }

        public GroupListPageViewModel()
        {
            var root = new EntryGroup() { Name = "Root" };
            var email = new EntryGroup(){Name="Email"};
            var gmailAccounts = new EntryGroup(){Name="Gmail"};
            gmailAccounts.AddEntry(new Entry() { Title = "Main gmail", Username = "Something@gmail.com" });
            email.EntryGroups.Add(gmailAccounts);
            email.AddEntry(new Entry() { Title = "Yahoo", Username = "Something@yahoo.com" });
            root.AddEntryGroup(email);

            var homebanking = new EntryGroup(){Name="Banking"};
            root.AddEntryGroup(homebanking);


            Root = root; 
                
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
        public string Name { get;  set; }

        public ObservableCollection<EntryGroup> EntryGroups { get; set; }
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
