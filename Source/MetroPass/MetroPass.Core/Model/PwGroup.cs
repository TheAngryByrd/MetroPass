using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Framework;

namespace MetroPass.Core.Model
{
    public class PwGroup : PwCommon
    {

        public XElement Element { get; private set; }
        public int IconId { get; set; }
        public string _name;
        public string Name { 
            get { 
                return _name; 
            } 
            set 
            {
                SetProperty(ref _name, value);
            } 
        }


        public ObservableCollection<PwEntry> Entries { get; set; }
        public ObservableCollection<PwGroup> SubGroups { get; set; }

        public ObservableCollection<object> AllTogetherNow
        {
            get
            {
                var temp = new ObservableCollection<object>();

                temp.AddRange(this.SubGroups);
                temp.AddRange(this.Entries);

                return temp;
            }
        }

        public PwGroup(XElement element)
        {
            Element = element;
            Entries = new ObservableCollection<PwEntry>();
            SubGroups = new ObservableCollection<PwGroup>();
        }
    }
}
