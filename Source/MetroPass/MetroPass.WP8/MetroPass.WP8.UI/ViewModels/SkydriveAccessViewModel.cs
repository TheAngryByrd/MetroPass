using System.Threading.Tasks;
using MetroPass.WP8.UI.Services.Cloud;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using ReactiveCaliburn;
using ReactiveUI;
using MetroPass.WP8.UI.Utils;
using Caliburn.Micro;
using MetroPass.WP8.UI.Views;

namespace MetroPass.WP8.UI.ViewModels
{
    public class SkydriveAccessViewModel : ReactiveScreen
    {      
        private LiveAuthClient authClient;
        private string Scopes = "wl.signin wl.skydrive_update wl.offline_access";
        private LiveLoginResult _liveLoginResult;
        private readonly INavigationService _navigationService;
        private readonly ICache _cache;
        public LiveConnectSession Session { get; internal set; }

        public ReactiveCommand LoginCommand { get; set; }

        private LiveLoginResult LiveLoginResult
        {
            get { return _liveLoginResult; }
            set { this.RaiseAndSetIfChanged(ref _liveLoginResult, value); }
        }
        
        public SkydriveAccessViewModel(INavigationService navigationService,
            ICache cache)
        {
            _cache = cache;
            _navigationService = navigationService;
            this.ObservableForProperty(vm => vm.LiveLoginResult).Subscribe(LiveLoginResultChanged); 
            
            LoginCommand = new ReactiveCommand();
            LoginCommand.Subscribe(Login);

            SignInIsEnabled = false;

            _navigationService.Navigated += _navigationService_Navigated;
        }

        private void LiveLoginResultChanged(IObservedChange<SkydriveAccessViewModel, LiveLoginResult> obj)
        {
            OnLogin(obj.Value);
        }

        void _navigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back
                && e.Content is SkydriveAccessView)
            {
                _navigationService.StopLoading();
                _navigationService.GoBack();
                _navigationService.Navigated -= _navigationService_Navigated;
            }
        }
    
        private async void Login(object arg)
        {
            LiveLoginResult = await authClient.LoginAsync(ParseScopeString(this.Scopes));
        }

        protected async override Task OnActivate()
        {
            if (authClient == null)
            {
                authClient = new LiveAuthClient(ApiKeys.SkyDriveClientId);

                IEnumerable<string> scopes = ParseScopeString(Scopes);

                try
                {
                    Task<LiveLoginResult> result = authClient.InitializeAsync(scopes);
                    LiveLoginResult = await result;                   
                }
                catch (Exception exception)
                {
                    //this.RaiseSessionChangedEvent(new LiveConnectSessionChangedEventArgs(exception));
                }
            }
        }

        private void OnLogin(LiveLoginResult loginResult)
        {
            Session = loginResult.Session;


            if (loginResult.Status == LiveConnectSessionStatus.Connected)
            {
                _cache.SkydriveSession = loginResult.Session;
                _navigationService.UriFor<BrowseCloudFilesViewModel>()
                    .WithParam(vm => vm.CloudProvider, CloudProvider.SkyDrive)
                    .WithParam(vm => vm.NavigationUrl, "/me/skydrive")
                    .Navigate();
            }
            else
            {
                _cache.SkydriveSession = null;
                SignInIsEnabled = true;
            }        
        }

        private IEnumerable<string> ParseScopeString(string scopesString)
        {
            return new List<string>(scopesString.Split(new [] {" "},StringSplitOptions.RemoveEmptyEntries));
        }

        private bool _signInIsEnabled;
        public bool SignInIsEnabled
        {
            get { return _signInIsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _signInIsEnabled, value); }
        }


    }
}
