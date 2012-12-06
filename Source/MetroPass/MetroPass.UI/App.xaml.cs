﻿using System;
using System.Collections.Generic;
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
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI
{
    public sealed partial class App
    { 
        private NinjectContainer _ninjectContainer;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnWindowCreated(Windows.UI.Xaml.WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);
            SettingsPane.GetForCurrentView().CommandsRequested += ShowPrivacyPolicy;
        }             

        void ShowPrivacyPolicy(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
           var privacyPolicyCommand = new SettingsCommand("privacyPolicy","Privacy Policy", a => LaunchPrivacyPolicyUrl());
           args.Request.ApplicationCommands.Add(privacyPolicyCommand);
        }

        async void LaunchPrivacyPolicyUrl()
        {
            Uri privacyPolicyUrl = new Uri("http://metropass.azurewebsites.net/Privacy-Policy");        
            var result = await Windows.System.Launcher.LaunchUriAsync(privacyPolicyUrl);
        }

        protected override void Configure()
        {
            base.Configure();

            _ninjectContainer = new NinjectContainer();
            _ninjectContainer.RegisterWinRTServices();

            _ninjectContainer.Kernel.Bind<IPageServices>().To<PageServices>();
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
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        protected override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            var navigationService = _ninjectContainer.Kernel.Get<INavigationService>();

            navigationService.UriFor<SearchResultsViewModel>().WithParam(vm => vm.QueryText, args.QueryText).Navigate();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            
            var navigationService = _ninjectContainer.Kernel.Get<INavigationService>();
            navigationService.NavigateToViewModel<LoadKdbViewModel>(args.Files[0]);
            //navigationService.UriFor<LoadKdbViewModel>().WithParam(vm => vm.Path, args.Files[0].Path).Navigate();
            //this.DisplayRootView<LoadKdbView>(args.Files[0]);
            //DisplayRootViewFor<LoadKdbViewModel>();
           // base.OnFileActivated(args);
        }
    }
}
