using MetroPass.UI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MetroPass.UI.Views
{
    public sealed partial class EntryEditView : Page, IPasswordErrorStateView
    {
        public EntryEditView()
        {
            this.InitializeComponent();

            //HACK: Needed due to lack of UpdateSourceTrigger in WinRT XAML
            PasswordTextBox.KeyUp += ForceUpdatePassword;
            PasswordSnapped.KeyUp += ForceUpdatePassword;
            ConfirmTextBox.KeyUp += ForceUpdateConfirmPassword;
            ConfirmSnapped.KeyUp += ForceUpdateConfirmPassword;
            Loaded += EntryEditView_Loaded;
        }

        void EntryEditView_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = ViewModel.MaskedPassword;
            ConfirmTextBox.Text = ViewModel.MaskedConfirm;
        }

        public void SetPasswordState(bool passwordsMatch)
        {
            if (passwordsMatch)
            {
                VisualStateManager.GoToState(this.ConfirmTextBox, "NoError", true);
                VisualStateManager.GoToState(this.ConfirmSnapped, "NoError", true);
            }
            else
            {
                VisualStateManager.GoToState(this.ConfirmTextBox, "Error", true);
                VisualStateManager.GoToState(this.ConfirmSnapped, "Error", true);
            }
        }
        private EntryEditViewModel ViewModel
        {
            get
            {
                return this.DataContext as EntryEditViewModel;
            }
        }

        private void ForceUpdatePassword(object sender, KeyRoutedEventArgs e)
        {
            var vm = this.DataContext as EntryEditViewModel;
            if (vm != null)
            {
                vm.Password = ((TextBox)sender).Text;
            }
        }

        private void ForceUpdateConfirmPassword(object sender, KeyRoutedEventArgs e)
        {
            var vm = this.DataContext as EntryEditViewModel;
            if (vm != null)
            {
                vm.Confirm = ((TextBox)sender).Text;
            }
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = ViewModel.Password;
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = ViewModel.MaskedPassword;
        }

        private void ConfirmTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ConfirmTextBox.Text = ViewModel.Confirm;
        }

        private void ConfirmTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfirmTextBox.Text = ViewModel.MaskedConfirm;
        }
    }
}