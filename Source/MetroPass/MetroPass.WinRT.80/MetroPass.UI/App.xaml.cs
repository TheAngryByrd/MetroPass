using System;
using System.Collections.Generic;
using Caliburn.Micro;
using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.ViewModels;
using MetroPass.UI.Views;
using MetroPass.WinRT.Infrastructure.PasswordGeneration;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.PasswordGeneration;
using Ninject;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MetroPass.UI
{
    public sealed partial class App
    { 

        private const string PrivacyPolicyUrl = "http://metropass.azurewebsites.net/Privacy-Policy";
        private const string SupportUrl = "https://metropasswin8.uservoice.com";

        private Bootstrapper _bootstrapper;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            ConventionManager.AddElementConvention<ToggleSwitch>(ToggleSwitch.IsOnProperty, "IsOn", "Toggled");    
            ConventionManager.AddElementConvention<Slider>(Slider.ValueProperty, "Value", "ValueChanged");
        }

        protected override void OnWindowCreated(Windows.UI.Xaml.WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            SettingsPane.GetForCurrentView().CommandsRequested += ConfigureSettings;
        }

        private void ConfigureSettings(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var settingsColor = App.Current.Resources["MainAppColor"] as SolidColorBrush;

            var aboutCommand = new SettingsCommand("about", "About MetroPass", a => DialogService.ShowSettingsFlyout<AboutSettingsViewModel>(GetBaseScreen(), headerBrush: settingsColor));
            args.Request.ApplicationCommands.Add(aboutCommand);

            var dataSource = _bootstrapper.GetInstance<IPWDatabaseDataSource>();
            var dbOptionsCommand = new SettingsCommand("databaseOptions", "Database Options", h =>
            {
                if (dataSource.PwDatabase != null)
                {
                    DialogService.ShowSettingsFlyout<DatabaseSettingsViewModel>(GetBaseScreen(), onClosed: SettingsClosed, headerBrush: settingsColor);
                }
                else
                {
                    DialogService.ShowSettingsFlyout<DatabaseClosedSettingsViewModel>(GetBaseScreen(), headerBrush: settingsColor);
                }
            });
            args.Request.ApplicationCommands.Add(dbOptionsCommand);

            var appOptionsCommand = new SettingsCommand("metroPassOptions", "MetroPass Options", h => DialogService.ShowSettingsFlyout<AppSettingsViewModel>(GetBaseScreen(), headerBrush: settingsColor));
            args.Request.ApplicationCommands.Add(appOptionsCommand);

            var privacyPolicyCommand = new SettingsCommand("privacyPolicy", "Privacy Policy", a => LaunchUrl(PrivacyPolicyUrl));
            args.Request.ApplicationCommands.Add(privacyPolicyCommand);

            var supportCommand = new SettingsCommand("support", "Support & Feedback", a => LaunchUrl(SupportUrl));
            args.Request.ApplicationCommands.Add(supportCommand);

        }
  
        private void SettingsClosed(DatabaseSettingsViewModel s, UIElement v)
        {
            SaveSettings(s, v);
        }

        private void SetAds(bool value)
        { 
            var baseScreen = GetBaseScreen();

            if (baseScreen != null)
            {
                baseScreen.IsAdVisible = value;         
            }
        }
  
        private BaseScreen GetBaseScreen()
        {
            var baseScreen = (RootFrame.Content as Page).DataContext as BaseScreen;
            return baseScreen;
        }

        private void SaveSettings(DatabaseSettingsViewModel settingsViewModel, UIElement _)
        {
            var dataSource = _bootstrapper.GetInstance<IPWDatabaseDataSource>();
            //The settings view model sets the properties directly on the IKdbTree, so we just need to save the database here
            dataSource.SavePwDatabase();
        }

        private async void LaunchUrl(string url)
        {                 
            var result = await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
        }

        protected override void Configure()
        {
            _bootstrapper = new Bootstrapper();
            _bootstrapper.Configure();
        }

        protected override object GetInstance(Type service, string key)
        {    
            return _bootstrapper.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _bootstrapper.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _bootstrapper.BuildUp(instance);
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _bootstrapper.PrepareViewFirst(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<StartPageView>();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            ApplicationData.Current.LocalSettings.Values["SuspendDate"] = DateTime.Now.ToString();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
        
        protected override void OnResuming(object sender, object e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SuspendDate"))
            {
                DateTime suspendedDate = DateTime.Parse(ApplicationData.Current.LocalSettings.Values["SuspendDate"].ToString());
                var locker = _bootstrapper.GetInstance<ILockingService>();
                locker.OnResumeLock(suspendedDate);
            }
        }

        protected override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            var dataSource = _bootstrapper.GetInstance<IPWDatabaseDataSource>();
            if (dataSource.PwDatabase == null)
            {
                Initialise();
                DisplayRootView<LoadKdbView>(args.QueryText);
            }
            else
            {
                var navigationService = _bootstrapper.GetInstance<INavigationService>();
                navigationService.UriFor<SearchResultsViewModel>().WithParam(vm => vm.QueryText, args.QueryText).Navigate();
            }
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            var dataSource = _bootstrapper.GetInstance<IPWDatabaseDataSource>();
            dataSource.StorageFile = args.Files[0] as StorageFile;
            DisplayRootView<LoadKdbView>();
        }
    }
}