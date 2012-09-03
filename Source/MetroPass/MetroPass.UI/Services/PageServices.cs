using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace MetroPass.UI.Services
{
    public class PageServices
    {
        public async Task Show(string title, string message)
        {
            var dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        public async Task Show(string message)
        {
            await Show(message, string.Empty);
        }

        public async Task<bool> EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
                await Show("Cannot unsnap the sample.");
            }

            return unsnapped;
        }
    }
}
