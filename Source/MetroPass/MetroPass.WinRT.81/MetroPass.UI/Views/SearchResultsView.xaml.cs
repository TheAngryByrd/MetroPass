using MetroPass.UI.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Views
{
    public sealed partial class SearchResultsView : Page
    {
        public SearchResultsView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.ApplicationViewStates.CurrentStateChanging += ApplicationViewStates_CurrentStateChanging;
        }

        protected override void OnNavigatingFrom(Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            this.ApplicationViewStates.CurrentStateChanging -= ApplicationViewStates_CurrentStateChanging;
        }

        void ApplicationViewStates_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            SetAppBarState(e.NewState.Name);
        }

        private void AppBarButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                VisualStateManager.GoToState(button, ApplicationView.Value.ToString(), true);
            }
        }

        //HACK: This is wired up to a Caliburn Action, but it's not working due to an issue with the Windows.UI.Interactivity library
        private void EntryAppBar_Closed(object sender, object e)
        {
            var vm = this.DataContext as SearchResultsViewModel;
            if (vm != null)
            {
                vm.DeselectItem();
            }
        }

        private void EntryAppBar_Opened(object sender, object e)
        {
            var vm = this.DataContext as SearchResultsViewModel;
        }

        private void SetAppBarState(string state)
        {
            VisualStateManager.GoToState(EditEntry, state, true);
            VisualStateManager.GoToState(CopyUsername, state, true);
            VisualStateManager.GoToState(CopyPassword, state, true);
            VisualStateManager.GoToState(OpenURL, state, true);
        }
    }
}