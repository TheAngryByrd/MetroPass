using MetroPass.UI.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.UI.Helpers;

namespace MetroPass.UI.ViewModels
{
    public class GroupListPageViewModel :BindableBase
    {
        List<A> Animals = new List<A> { new A("Cat"),new A("Dog"), new A("Moose") };
        List<B> Numbers = new List<B> { new B(15), new B(40), new B(80) };
        ObservableCollection<object> list = new ObservableCollection<object>();
        public ObservableCollection<object> MyList
        {
            get
            {

                return list;
            }
        }

        public GroupListPageViewModel()
        {
            
                var xlist = new ObservableCollection<object>();
                
                xlist.AddRange(Animals);
                xlist.AddRange(Numbers.Cast<object>());

                SetProperty(ref list, xlist);
                
        }
    }
    public class A
    {
        public string Name { get; private set; }
        public A(string value)
        {
            this.Name = value;
        }
    }
    public class B
    {
        public int Value { get; private set; }
        public B(int value)
        {
            this.Value = value;
        }
    }
}
