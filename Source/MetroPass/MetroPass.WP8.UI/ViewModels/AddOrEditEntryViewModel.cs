using System;
using System.Collections.Generic;
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
        public PwEntry PwEntry {
            get {
                return _pwEntry;
            }
            set {
                this.RaiseAndSetIfChanged(ref _pwEntry, value);
            }
        }

        public AddOrEditEntryViewModel()
        {
            this.ObservableForProperty(vm => vm.EntryUuid).Subscribe(GetEntry);
        }

        public void GetEntry(object obj)
        {
            PwEntry = PWDatabaseDataSource.Instance.PwDatabase.Tree.FindEntryByUuid(EntryUuid);
        }


    }
}
