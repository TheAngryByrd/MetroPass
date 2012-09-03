using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.UI.ViewModels
{
    public class EntryGroupListPageViewModel : BindableBase
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

        public EntryGroupListPageViewModel(DataModel.EntryGroup entryGroup)
        {
            // TODO: Complete member initialization
            this._root = entryGroup;
        }
    }
}
