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
    }
}