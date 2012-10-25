using System.Collections.Generic;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.UI.Common;
using System;
using System.Linq;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.ViewModels;
using MetroPass.UI.Views;
using Windows.ApplicationModel;
using Ninject;

namespace MetroPass.UI
{

    public sealed partial class App
    { 
        private NinjectContainer ninjectContainer;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void Configure()
        {
            base.Configure();

            ninjectContainer = new NinjectContainer(RootFrame);
            ninjectContainer.RegisterWinRTServices();

            ninjectContainer.Kernel.Bind<IPageServices>().To<PageServices>();
            ninjectContainer.Kernel.Bind<IClipboard>().To<MetroClipboard>();
            ninjectContainer.Kernel.Bind<IKdbTree>().ToMethod(c => PWDatabaseDataSource.Instance.PwDatabase.Tree);
        }

        protected override object GetInstance(Type service, string key)
        {    
            var instance = ninjectContainer.Kernel.Get(service, key); 
            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return ninjectContainer.Kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            ninjectContainer.Kernel.Inject(instance);
        }

        protected override Type GetDefaultView()
        {
            return typeof(StartPageView);
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
            var navigationService = ninjectContainer.Kernel.Get<INavigationService>();

            navigationService.UriFor<SearchResultsViewModel>().WithParam(vm => vm.QueryText, args.QueryText).Navigate();
        }
    }
}
