using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MetroPass.WP8.UI.ViewModels;
using MetroPass.WP8.UI.Views;
using Ninject;
using ReactiveUI;
using ReactiveUI.Mobile;

public static class ReactiveUINavigateExtentions
{
    public static void Navigate<T>(this INavigateCommand command)
    {
        var VM = RxApp.DependencyResolver.GetService<T>();
        command.Execute(VM);
    }
}

namespace MetroPass.WP8.UI
{
    
        [DataContract]
        public class AppBootstrapper : ReactiveObject, IApplicationRootState
        {
            [DataMember]
            RoutingState _Router;

            public IRoutingState Router
            {
                get { return _Router; }
                set { _Router = (RoutingState)value; } // XXX: This is dumb.
            }

            [IgnoreDataMember]
            private readonly NinjectDependencyResolver _ninjectResolver;

            public AppBootstrapper(NinjectDependencyResolver resolver)
            {
                _ninjectResolver = resolver;
                Router = new RoutingState();

                _ninjectResolver.Kernel.Bind<IViewFor<SkydriveAccessViewModel>>().To<SkydriveAccessView>();

                _ninjectResolver.Kernel.Bind<IApplicationRootState>().ToConstant(this);

                _ninjectResolver.Kernel.Bind<IScreen>().ToConstant(this);


                Router.Navigate.Navigate<SkydriveAccessViewModel>();
            }
        }

        public class NinjectDependencyResolver : IDependencyResolver
        {
            private readonly IDependencyResolver _defaultResolver;
            public IKernel Kernel { get; protected set; }

            public NinjectDependencyResolver(IDependencyResolver defaultResolver)
            {
                _defaultResolver = defaultResolver;
                Kernel = new StandardKernel();
             
            }

            public object GetService(Type serviceType, string contract = null) {
                try
                {
                    return Kernel.Get(serviceType, contract);
                }
                catch (Exception)
                {
                    
                }

                try
                {
                   return _defaultResolver.GetService(serviceType, contract);
                }
                catch (Exception)
                {                    
                    throw;
                }
            }

            public IEnumerable<object> GetServices(Type serviceType, string contract = null) {

                IEnumerable<object> retVal = null;


                try
                {
                    retVal = Kernel.GetAll(serviceType, contract);
                }
                catch (Exception)
                {

                }
                if (retVal != null && retVal.Any())
                {
                    return retVal;
                }

                try
                {
                    retVal = _defaultResolver.GetServices(serviceType, contract);
                }
                catch (Exception)
                {
                    throw;
                }

                if (retVal != null && retVal.Any())
                {
                    return retVal;
                }

                throw new ArgumentException();
            }

            public void Dispose() {
                Kernel.Dispose();
                _defaultResolver.Dispose();
            }
        }
    
}
