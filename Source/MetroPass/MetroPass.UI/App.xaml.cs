using System;
using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.ViewModels;
using MetroPass.UI.Views;
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
        private NinjectContainer _ninjectContainer;

        private const string PrivacyPolicyUrl = "http://metropass.azurewebsites.net/Privacy-Policy";
        private const string SupportUrl = "https://metropasswin8.uservoice.com";

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnWindowCreated(Windows.UI.Xaml.WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            SettingsPane.GetForCurrentView().CommandsRequested += ConfigureSettings;
        }

        private void ConfigureSettings(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var settingsColor = App.Current.Resources["MainAppColor"] as SolidColorBrush;

            var privacyPolicyCommand = new SettingsCommand("privacyPolicy","Privacy Policy", a => LaunchUrl(PrivacyPolicyUrl));
            args.Request.ApplicationCommands.Add(privacyPolicyCommand);

            var supportCommand = new SettingsCommand("support", "Support", a => LaunchUrl(SupportUrl));
            args.Request.ApplicationCommands.Add(supportCommand);

            var optionsCommand = new SettingsCommand("metroPassOptions", "Options", h =>
            {
                if (PWDatabaseDataSource.Instance.PwDatabase != null) {
                    DialogService.ShowSettings<SettingsViewModel>(onClosed: SaveSettings, headerBrush: settingsColor);
                } else {
                    DialogService.ShowSettings<DatabaseClosedSettingsViewModel>(headerBrush: settingsColor);
                }
            });
            args.Request.ApplicationCommands.Add(optionsCommand);
        }

        private void SaveSettings(SettingsViewModel settingsViewModel, UIElement _)
        {
            //The settings view model sets the properties directly on the IKdbTree, so we just need to save the database here
            PWDatabaseDataSource.Instance.SavePwDatabase();
        }

 

        private async void LaunchUrl(string url)
        {                 
            var result = await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
        }

        protected override void Configure()
        {
            base.Configure();

            _ninjectContainer = new NinjectContainer();
            _ninjectContainer.RegisterWinRTServices();

            _ninjectContainer.Kernel.Bind<IPageServices>().To<PageServices>();
            _ninjectContainer.Kernel.Bind<ILockingService>().To<LockingService>();
            _ninjectContainer.Kernel.Bind<IClipboard>().To<MetroClipboard>();
            _ninjectContainer.Kernel.Bind<IKdbTree>().ToMethod(c => PWDatabaseDataSource.Instance.PwDatabase.Tree);
        }

        protected override object GetInstance(Type service, string key)
        {    
            var instance = _ninjectContainer.Kernel.Get(service, key); 
            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _ninjectContainer.Kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            _ninjectContainer.Kernel.Inject(instance);
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _ninjectContainer.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<StartPageView>();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
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
                var locker =_ninjectContainer.Kernel.Get<ILockingService>();
                locker.OnResumeLock(suspendedDate);
            }
        }



        protected override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            var navigationService = _ninjectContainer.Kernel.Get<INavigationService>();

            navigationService.UriFor<SearchResultsViewModel>().WithParam(vm => vm.QueryText, args.QueryText).Navigate();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {           
            PWDatabaseDataSource.Instance.StorageFile = args.Files[0] as StorageFile;
            DisplayRootView<LoadKdbView>();
        }
    }
}