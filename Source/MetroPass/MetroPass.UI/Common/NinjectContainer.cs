using Caliburn.Micro;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Common
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
}
