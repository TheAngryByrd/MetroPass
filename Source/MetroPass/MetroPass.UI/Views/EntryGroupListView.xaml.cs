using MetroPass.UI.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Views
{
    public sealed partial class EntryGroupListView : Page, IHaveAppBar
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

        private void AppBarButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                VisualStateManager.GoToState(button, ApplicationView.Value.ToString(), true);
            }
        }

        public void SetAppBarState(string state)
        {
            VisualStateManager.GoToState(EditGroup, state, true);
            VisualStateManager.GoToState(DeleteGroup, state, true);
            VisualStateManager.GoToState(AddEntry, state, true);
            VisualStateManager.GoToState(AddGroup, state, true);

            VisualStateManager.GoToState(EditEntry, state, true);
            VisualStateManager.GoToState(DeleteEntry, state, true);
            VisualStateManager.GoToState(CopyUsername, state, true);
            VisualStateManager.GoToState(CopyPassword, state, true);
            VisualStateManager.GoToState(OpenURL, state, true);
        }
    }
}