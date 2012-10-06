using System;
using MetroPass.UI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Views
{
    public sealed partial class LoadKdbView : Page, ILoadKdbView
    {
        public LoadKdbView()
        {
            this.InitializeComponent();
        }

        public void FocusPassword()
        {
            this.Password.Focus(FocusState.Programmatic);
        }

        private void Password_KeyUp_1(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var vm = this.DataContext as LoadKdbViewModel;
                if (vm != null)
                {
                    vm.OpenDatabase();
                }
            }
        }
    }
}