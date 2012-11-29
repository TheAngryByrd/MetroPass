using MetroPass.UI.ViewModels;
using Windows.UI.Xaml.Controls;

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
            }
        }
    }
}