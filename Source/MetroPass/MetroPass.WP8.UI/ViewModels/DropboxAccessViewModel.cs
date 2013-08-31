using Caliburn.Micro;
using DropNetRT;
using MetroPass.WP8.UI.Services.Cloud;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using MetroPass.WP8.UI.Views;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DropboxAccessViewModel : ReactiveScreen
    {
        string CallBack = "http://metropass.azurewebsites.net/";

        private DropNetClient _client;

        private readonly INavigationService _navigationService;

        public DropboxAccessViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _client = new DropNetClient(
                ApiKeys.DropBoxKey, ApiKeys.DropBoxSecret);
        }

        public async void Loaded(RoutedEventArgs e)
        {
            var view = GetView() as DropboxAccessView;
            
            var requestToken = await _client.GetRequestToken();
            var url = _client.BuildAuthorizeUrl(requestToken, CallBack);
            
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                view.browser.Navigate(new Uri(url)));
        }

        public async void Navigating(NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(CallBack))
                await CheckToken();
        }

        private async Task CheckToken()
        {
            var accessToken = await _client.GetAccessToken();

            Cache.Instance.DropboxUserToken = accessToken.Token;
            Cache.Instance.DropboxUserSecret = accessToken.Secret;

            _navigationService.UriFor<SkydriveBrowseFilesViewModel>()
                    .WithParam(vm => vm.CloudProvider, CloudProvider.Dropbox)
                    .WithParam(vm => vm.NavigationUrl, "/")
                    .Navigate();


        }
    }
}
