using System.Threading.Tasks;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using Microsoft.Live.Controls;
using ReactiveUI;
using MetroPass.WP8.UI.Utils;
using Caliburn.Micro;

namespace MetroPass.WP8.UI.ViewModels
{
    public class SkydriveAccessViewModel : ReactiveScreen
    {      
        private LiveAuthClient authClient;
        private string Scopes = "wl.signin wl.basic wl.skydrive_update wl.offline_access";
        private LiveLoginResult _liveLoginResult;
        private readonly INavigationService _navigationService;


        public LiveConnectSession Session { get; internal set; }

        public ReactiveCommand LoginCommand { get; set; }

        private LiveLoginResult LiveLoginResult
        {
            get { return _liveLoginResult; }
            set { this.RaiseAndSetIfChanged(ref _liveLoginResult, value); }
        }

        public SkydriveAccessViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.ObservableForProperty(vm => vm.LiveLoginResult).Subscribe(LiveLoginResultChanged); 


            LoginCommand = new ReactiveCommand();
            LoginCommand.Subscribe(Login);

            SignInIsEnabled = false;
        }

        private void LiveLoginResultChanged(IObservedChange<SkydriveAccessViewModel, LiveLoginResult> obj)
        {
            OnLogin(obj.Value);
        }

    
        private async void Login(object arg)
        {
            LiveLoginResult = await authClient.LoginAsync(ParseScopeString(this.Scopes));
        }

        protected async override void OnActivate()
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
                Cache.Instance.SkydriveSession = loginResult.Session;
                _navigationService.UriFor<SkydriveBrowseFilesViewModel>().Navigate();
            }
            else
            {
                Cache.Instance.SkydriveSession = null;
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
