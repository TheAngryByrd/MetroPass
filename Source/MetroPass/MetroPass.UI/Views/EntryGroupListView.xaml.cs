using Caliburn.Micro;
using MetroPass.UI.ViewModels;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Views
{
    public sealed partial class EntryGroupListView : Page
    {
        private bool isCtrlKeyPressed;

        public EntryGroupListView()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (e.VirtualKey == VirtualKey.Control) isCtrlKeyPressed = true;
            else if (isCtrlKeyPressed)
            {

                var entryListVM = ViewModelLocator.LocateForView(this) as EntryGroupListViewModel;
                
                switch (e.VirtualKey)
                {
                    case VirtualKey.B: entryListVM.CopyUsername(); break;
                    case VirtualKey.C: entryListVM.CopyPassword(); break;
          
                }
            }
        }

        void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (e.VirtualKey == VirtualKey.Control)
                this.isCtrlKeyPressed = false;
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
                vm.IsAdVisible = false;
            }
        }

        private void AppBarButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                VisualStateManager.GoToState(button, ApplicationView.Value.ToString(), true);
            }
        }

        private void SetAppBarState(string state)
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

        private void SemanticZoom_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            var vm = this.DataContext as EntryGroupListViewModel;
            if (e.IsSourceZoomedInView)
            {
                vm.IsAdVisible = false;
            }
            else
            {
                vm.IsAdVisible = true;
            }
        }


  

    }
}