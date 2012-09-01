using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.UI.Helpers
{
    public static class Extend
    {
        // Extend ObservableCollection<T> Class
        public static void AddRange<U>(this ObservableCollection<object> o, IEnumerable<U> items)
        {
            foreach (var item in items)
            {
                o.Add(item as object);
            }
        }
    }
}
