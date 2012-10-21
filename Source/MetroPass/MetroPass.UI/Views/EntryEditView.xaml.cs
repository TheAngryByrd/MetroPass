using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Views
{
    public sealed partial class EntryEditView : Page, IEntryEditView
    {
        public EntryEditView()
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