using System;
using System.Windows;
using Caliburn.Micro;
using DropNetRT;
using Microsoft.Phone.Controls;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI.Views
{
    public partial class DropboxAccessView : PhoneApplicationPage
    {
        string CallBack = "http://metropass.azurewebsites.net/";

        private readonly INavigationService _navigationService;

        public DropboxAccessView()
        {
          
            InitializeComponent();
            _client = new DropNetClient(
                ApiKeys.DropBoxKey,ApiKeys.DropBoxSecret);
        }

        private readonly DropNetClient _client;

        private async Task CheckToken()
        {
            var accessToken = await _client.GetAccessToken();

            var data = await _client.GetMetaData("/");
            var data2 = data;
        }

        private void ShowError()
        {
           
        }

        private void browser_LoadCompleted(object sender,
            System.Windows.Navigation.NavigationEventArgs e)
        {
    
        }

        private async void browser_Loaded(object sender, RoutedEventArgs e)
        {

            var requestToken = await _client.GetRequestToken();
            var url = _client.BuildAuthorizeUrl(requestToken, CallBack);
            Dispatcher.BeginInvoke(() =>
                browser.Navigate(new Uri(url)));
        }

        private async void browser_Navigating(object sender, NavigatingEventArgs e)
        {
       

            if (e.Uri.ToString().StartsWith(CallBack))
                await CheckToken();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("54a0ce26bb7646e582a52b4faabbf8289b0f2b36e09d9ee8e9db9c62eb3eb042");
        }
    }
}