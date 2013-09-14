using Caliburn.Micro;
using DropNetRT;
using MetroPass.WP8.UI.Services.Cloud;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.Views;
using Microsoft.Phone.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using ReactiveCaliburn;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DropboxAccessViewModel : ReactiveScreen
    {
        string CallBack = "http://metropass.azurewebsites.net/";

        private DropNetClient _client;
        private readonly INavigationService _navigationService;
        private readonly ICache _cache;

        public DropboxAccessViewModel(INavigationService navigationService,
            ICache cache)
        {
            _cache = cache;
            _navigationService = navigationService;
            _client = new DropNetClient(
                ApiKeys.DropBoxKey, ApiKeys.DropBoxSecret);
            _navigationService.Navigated += _navigationService_Navigated;
        }

        void _navigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
              if(e.NavigationMode == System.Windows.Navigation.NavigationMode.Back
                  && e.Content is DropboxAccessView)
                {
                    _navigationService.StopLoading();
                    _navigationService.GoBack();
                    _navigationService.Navigated -= _navigationService_Navigated;
              }
        }

        protected async override void OnActivate()
        {            
            var view = GetView() as DropboxAccessView;
            view.browser.Visibility = Visibility.Collapsed;

            var requestToken = await _client.GetRequestToken();
            var url = _client.BuildAuthorizeUrl(requestToken, CallBack);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                view.browser.Navigate(new Uri(url)));
        }

        protected override void OnDeactivate(bool close)
        {
           
        }

        public void LoadCompleted()
        {
            var view = GetView() as DropboxAccessView;
            view.browser.Visibility = Visibility.Visible;
        }

        public async void Navigating(NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(CallBack))
                await CheckToken();
        }

        private async Task CheckToken()
        {
            var accessToken = await _client.GetAccessToken();

            _cache.DropboxUserToken = accessToken.Token;
            _cache.DropboxUserSecret = accessToken.Secret;

            _navigationService.UriFor<BrowseCloudFilesViewModel>()
                    .WithParam(vm => vm.CloudProvider, CloudProvider.Dropbox)
                    .WithParam(vm => vm.NavigationUrl, "/")
                    .Navigate();

        }

       
    }
}
