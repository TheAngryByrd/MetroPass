using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.UI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Common
{
    public class EntryGroupListViewHeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate AdvertTemplate { get; set; }
      
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
          {
          
              return NormalTemplate;
          }
    }
}
