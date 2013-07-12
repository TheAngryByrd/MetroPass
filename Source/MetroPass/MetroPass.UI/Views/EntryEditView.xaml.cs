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

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.FontFamily = App.NormalFont;
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.FontFamily = App.PasswordFont;
        }

        private void ConfirmTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ConfirmTextBox.FontFamily = App.NormalFont;
        }

        private void ConfirmTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfirmTextBox.FontFamily = App.PasswordFont;
        }
    }
}