using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.UI.ViewModels
{
    public class GroupListPageViewModel : BindableBase
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



        public GroupListPageViewModel(EntryGroup root)
        {
            Root = root;
        }


    }
 
}
