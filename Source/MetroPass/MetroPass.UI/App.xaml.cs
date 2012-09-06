using System.Collections.Generic;
using Caliburn.Micro;
using MetroPass.UI.Common;
using System;
using System.Linq;
using MetroPass.UI.Services;
using MetroPass.UI.Views;
using Windows.ApplicationModel;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Windows.UI.Xaml.Controls;


namespace MetroPass.UI
{
    public class NinjectContainer
    {
        public IKernel Kernel { get; private set; }
        private readonly Frame _rootFrame;
        private SimpleContainer simple = new SimpleContainer();

        public NinjectContainer(Frame rootFrame)
        {
            var settings = new NinjectSettings();
            settings.LoadExtensions = false;
            Kernel = new StandardKernel(settings);
            _rootFrame = rootFrame;
        }

        public void RegisterWinRTServices(bool treatViewAsLoaded = false)
        {
            Kernel.Rebind<SimpleContainer>().ToConstant<SimpleContainer>(simple);
            var navService = new FrameAdapter(_rootFrame, treatViewAsLoaded);
            Kernel.Rebind<INavigationService>().ToMethod<FrameAdapter>(x => navService);
            Kernel.Rebind<IEventAggregator>().To<EventAggregator>();
        }
    }

    public sealed partial class App
    {
        //private WinRTContainer container;
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

        }

        protected override object GetInstance(Type service, string key)
       {
    
            var instance = ninjectContainer.Kernel.TryGet(service, key);
 
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
