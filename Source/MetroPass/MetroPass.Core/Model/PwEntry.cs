using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetroPass.Core.Model
{
    public class PwEntry : PwCommon
    {
        public PwEntry(XElement element)
        {
            Element = element;
        }

        public IEnumerable<XElement> Meta
        {
            get
            {
                return Element.Elements("String");
            }
        }

        private XElement GetElement([CallerMemberName] String propertyName = null)
        {
            var titleElement = Meta.First(a => a.Element("Key").Value.ToLower() == propertyName.ToLower());
            return titleElement.Element("Value");
        }
           
        public string Title
        {
            get 
            {
                return GetElement().Value;
            }
            set 
            { 
                GetElement().Value = value;
                NotifyPropertyChanged(); 
            }
        }

  

        public string Username
        {
            get
            {
                return GetElement().Value;
            }
            set
            {
                GetElement().Value = value;
                NotifyPropertyChanged();
            }
        }

 

        public string Password
        {
            get
            {
                return GetElement().Value;
            }
            set
            {
                GetElement().Value = value;
                NotifyPropertyChanged();
            }
        }



        public string Url
        {
            get
            {
                return GetElement().Value;
            }
            set
            {
                GetElement().Value = value;
                NotifyPropertyChanged();
            }
        }



        public string Notes
        {
            get
            {
                return GetElement().Value;
            }
            set
            {
                GetElement().Value = value;
                NotifyPropertyChanged();
            }
        }



  
    }
}
