using MetroPass.UI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MetroPass.UI.Views
{
    public sealed partial class AddEntryView : Page, IPasswordErrorStateView
    {
        public AddEntryView()
        {
            this.InitializeComponent();

            //HACK: Needed due to lack of UpdateSourceTrigger in WinRT XAML
            Password.KeyUp += ForceUpdatePassword;
            PasswordSnapped.KeyUp += ForceUpdatePassword;
            Confirm.KeyUp += ForceUpdateConfirmPassword;
            ConfirmSnapped.KeyUp += ForceUpdateConfirmPassword;
        }

        public void SetPasswordState(bool passwordsMatch)
        {
            if (passwordsMatch)
            {
                VisualStateManager.GoToState(this.Confirm, "NoError", true);
                VisualStateManager.GoToState(this.ConfirmSnapped, "NoError", true);
            }
            else
            {
                VisualStateManager.GoToState(this.Confirm, "Error", true);
                VisualStateManager.GoToState(this.ConfirmSnapped, "Error", true);
            }
        }

        private void ForceUpdatePassword(object sender, KeyRoutedEventArgs e)
        {
            var vm = this.DataContext as AddEntryViewModel;
            if (vm != null)
            {
                vm.Password = ((TextBox)sender).Text;
            }
        }

        private void ForceUpdateConfirmPassword(object sender, KeyRoutedEventArgs e)
        {
            var vm = this.DataContext as AddEntryViewModel;
            if (vm != null)
            {
                vm.Confirm = ((TextBox)sender).Text;
            }
        }
    }
}