using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Metropass.Core.PCL.Model;
using ReactiveUI;

namespace MetroPass.WP8.UI.ViewModels
{
    public class AddOrEditEntryViewModel : ReactiveScreen
    {
        private PwGroup _pwGroup;

        public AddOrEditEntryViewModel()
        {
            AddCustomFieldCommand = new ReactiveCommand();
            //AddCustomFieldCommand.Subscribe(AddCustomField);
        }

        protected override void OnActivate()
        {
            GetEntry();
        }

        private string _parentGroupUuid;
        public string ParentGroupUuid
        {
            get
            {
                return _parentGroupUuid;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _parentGroupUuid, value);
            }
        }

        public PwGroup PwGroup
        {
            get
            {
                return _pwGroup;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _pwGroup, value);
            }
        }

        private string _entryUuid;
        public string EntryUuid
        {
            get
            {
                return _entryUuid;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _entryUuid, value);
            }
        }

        private PwEntry _pwEntry;
        public PwEntry PwEntry
        {
            get { return _pwEntry; }
            set { this.RaiseAndSetIfChanged(ref _pwEntry, value); }
        }

        public void GetEntry()
        {
            PwGroup = PWDatabaseDataSource.Instance.PwDatabase.Tree.FindGroupByUuid(ParentGroupUuid);
            if (EntryUuid != null)
            {
                PwEntry = PwGroup.Entries.SingleOrDefault(e => e.UUID == EntryUuid);// PWDatabaseDataSource.Instance.PwDatabase.Tree.FindEntryByUuid(EntryUuid);
                Title = PwEntry.Title;
                Username = PwEntry.Username;
                Password = PwEntry.Password;
                Url = PwEntry.Url;
                Notes = PwEntry.Notes;
                CustomFields = new ObservableCollection<FieldViewModel>(
                    PwEntry.CustomFields.
                    Select(cf => new FieldViewModel(cf)));
            }
            
        }

        public string PageTitle
        {
            get{ return "entry"; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        public string _username;
        public string Username
        {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        public string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public string _url;
        public string Url
        {
            get { return _url; }
            set { this.RaiseAndSetIfChanged(ref _url, value); }
        }

        public string _notes;
        public string Notes
        {
            get { return _notes; }
            set { this.RaiseAndSetIfChanged(ref _notes, value); }
        }

        public ObservableCollection<FieldViewModel> CustomFields { get; set; }

        public ReactiveCommand AddCustomFieldCommand { get; set; }

        public void AddField()
        {
            CustomFields.Add(new FieldViewModel(Field.New()));
        }

        public void Save()
        {
            foreach(var field in CustomFields)
            {
                field.Persist();
            }

            var fields = CustomFields.Select(f => f._field);

            PwEntry.Title = Title;
            PwEntry.Username = Username;
            PwEntry.Password = Password;
            PwEntry.Url = Url;
            PwEntry.Notes = Notes;
            PwEntry.AddCustomFields(fields);
        }
    }

    public class FieldViewModel : ReactivePropertyChangedBase
    {
        public readonly Field _field;

        public FieldViewModel(Field field)
        {
            _field = field;
            Name = field.Name;
            Value = field.Value;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _value;
        public string Value
        {
            get{ return _value; }
            set { this.RaiseAndSetIfChanged(ref _value, value); }
        }

        public void Persist()
        {
            _field.Name = Name;
            _field.Value = Value;
        }
    }
}
