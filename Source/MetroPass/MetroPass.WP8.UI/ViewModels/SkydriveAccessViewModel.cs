using System.Threading.Tasks;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using Microsoft.Live.Controls;
using ReactiveUI;

namespace MetroPass.WP8.UI.ViewModels
{
    public class SkydriveAccessViewModel : ReactiveScreen
    {      
        private LiveAuthClient authClient;
        private string Scopes = "wl.signin wl.basic wl.skydrive_update wl.offline_access";
        private LiveLoginResult _liveLoginResult;

        public LiveConnectSession Session { get; internal set; }

        public ReactiveCommand LoginCommand { get; set; }

        private LiveLoginResult LiveLoginResult
        {
            get { return _liveLoginResult; }
            set { this.RaiseAndSetIfChanged(ref _liveLoginResult, value); }
        }

        public SkydriveAccessViewModel()
        {
            this.ObservableForProperty(vm => vm.LiveLoginResult).Subscribe(LiveLoginResultChanged); 


            LoginCommand = new ReactiveCommand();
            LoginCommand.Subscribe(Login);
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

                this.IsEnabled = true;
            }
        }

        private void OnLogin(LiveLoginResult loginResult)
        {
            Session = loginResult.Session;

            var sessionChangedArgs =
                new LiveConnectSessionChangedEventArgs(loginResult.Status, loginResult.Session);

            this.RaiseSessionChangedEvent(sessionChangedArgs);           
        }

        private void RaiseSessionChangedEvent(LiveConnectSessionChangedEventArgs sessionChangedArgs)
        {
            if (sessionChangedArgs.Status == LiveConnectSessionStatus.Connected)
            {
                //MainPage.session = e.Session;
                //this.NavigationService.Navigate(new Uri("/Index.xaml", UriKind.Relative));
            }
            else
            {
                
            }
        }

       

        private IEnumerable<string> ParseScopeString(string scopesString)
        {
            return new List<string>(scopesString.Split(new [] {" "},StringSplitOptions.RemoveEmptyEntries));
        }

        public bool IsEnabled { get; set; }


    }
}
