using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Framework;
using System.Xml;

namespace MetroPass.Core.Model
{
    public class PwGroup : PwCommon
    {



    
        public string Name { 
            get {
                return Element.Element().Value; 
            } 
            set 
            {
                Element.Element().Value = value;
                NotifyPropertyChanged();      
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
