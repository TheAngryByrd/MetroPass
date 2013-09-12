using Caliburn.Micro;
using Ninject;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Common
{
    public class NinjectContainer
    {
        public IKernel Kernel { get; private set; }
        private SimpleContainer simple = new SimpleContainer();

        public NinjectContainer()
        {
            var settings = new NinjectSettings();            
            settings.LoadExtensions = false;
            Kernel = new StandardKernel(settings);
        }

        public void RegisterWinRTServices(bool treatViewAsLoaded = false)
        {
            Kernel.Rebind<SimpleContainer>().ToConstant<SimpleContainer>(simple);
            Kernel.Rebind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
        }

        public void RegisterNavigationService(Frame rootFrame, bool treatViewAsLoaded = false)
        {
            var navService = new FrameAdapter(rootFrame, treatViewAsLoaded);
            Kernel.Rebind<INavigationService>().ToMethod<FrameAdapter>(x => navService);
        }
    }
}