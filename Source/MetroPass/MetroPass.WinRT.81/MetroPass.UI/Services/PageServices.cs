using System;
using System.Linq;
using System.Threading.Tasks;
using NotificationsExtensions.ToastContent;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace MetroPass.UI.Services
{
    public class PageServices : IPageServices
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


        public void Toast(string message)
        {
            var template = ToastContentFactory.CreateToastText01();
            template.TextBodyWrap.Text = message;
            //var toast = new ToastNotification();
            //ToastNotificationManager.CreateToastNotifier().Show(toast);
            ToastNotification toast = template.CreateNotification();

            // If you have other applications in your package, you can specify the AppId of
            // the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
