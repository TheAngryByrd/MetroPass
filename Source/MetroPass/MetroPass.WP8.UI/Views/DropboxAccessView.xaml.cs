using System.Windows;
using Microsoft.Phone.Controls;

namespace MetroPass.WP8.UI.Views
{
    public partial class DropboxAccessView : PhoneApplicationPage
    {
        public DropboxAccessView()
        {           
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("54a0ce26bb7646e582a52b4faabbf8289b0f2b36e09d9ee8e9db9c62eb3eb042");
        }
    }
}