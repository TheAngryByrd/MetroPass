using System.Collections.Generic;
using System.Linq;
using MetroPass.UI.ViewModels;
using MetroPass.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MetroPass.UI.Views
{
    public sealed partial class EntryGroupListView : Page
    {
        public EntryGroupListView()
        {
            this.InitializeComponent();
        }
        
        //HACK: This is wired up to a Caliburn Action, but it's not working due to an issue with the Windows.UI.Interactivity library
        private void EntryAppBar_Closed(object sender, object e)
        {
            var vm = this.DataContext as EntryGroupListViewModel;
            if (vm != null)
            {
                vm.DeselectItem();
                vm.IsAdVisible = true;
            }  
        }

        private void EntryAppBar_Opened(object sender, object e)
        {
            var vm = this.DataContext as EntryGroupListViewModel;
            if (vm != null)
            {
                vm.IsAdVisible = true;
            }
            vm.IsAdVisible = false;
        }          
    }
}