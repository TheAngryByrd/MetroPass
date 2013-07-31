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
            PasswordTextBox.FontFamily = App.PasswordFont;
            ConfirmTextBox.FontFamily = App.PasswordFont;
            PasswordSnapped.FontFamily = App.PasswordFont;
            ConfirmSnapped.FontFamily = App.PasswordFont;
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
            get { return this.DataContext as EntryEditViewModel; }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).FontFamily = App.NormalFont;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).FontFamily = App.PasswordFont;
        }
    }
}