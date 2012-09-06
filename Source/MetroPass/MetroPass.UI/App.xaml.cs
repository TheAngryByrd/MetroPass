using System.Collections.Generic;
using Caliburn.Micro;
using MetroPass.UI.Common;
using System;
using System.Linq;
using MetroPass.UI.Services;
using MetroPass.UI.Views;
using Windows.ApplicationModel;
using Ninject;


namespace MetroPass.UI
{
    public sealed partial class App
    {
        private WinRTContainer container;
        private IKernel ninjectContainer;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void Configure()
        {
            base.Configure();

            var kernel = new StandardKernel();

     
            
            container = new WinRTContainer(RootFrame);
            container.RegisterWinRTServices();
            
            container.PerRequest<IPageServices, PageServices>();
        
            
        }

        protected override object GetInstance(Type service, string key)
        {

           
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override Type GetDefaultView()
        {
            return typeof(LoadKdbView);
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
    }
}
