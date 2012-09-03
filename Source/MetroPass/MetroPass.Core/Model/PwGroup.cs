using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public PwGroup(XElement element)
        {
            Element = element;
            Entries = new ObservableCollection<PwEntry>();
            SubGroups = new ObservableCollection<PwGroup>();
        }
    }
}
