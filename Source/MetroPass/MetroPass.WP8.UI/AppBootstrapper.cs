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

                _ninjectResolver.Kernel.Bind<IViewFor<DatabaseListViewModel>>().To<DatabaseListView>();
                _ninjectResolver.Kernel.Bind<IViewFor<SkydriveAccessViewModel>>().To<SkydriveAccessView>();
         

                _ninjectResolver.Kernel.Bind<IApplicationRootState>().ToConstant(this);

                _ninjectResolver.Kernel.Bind<IScreen>().ToConstant(this);


                Router.Navigate.Navigate<DatabaseListViewModel>();
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

                var retval = _defaultResolver.GetService(serviceType, contract);

                if (retval != null)
                    return retval;

                try
                {
                    return Kernel.Get(serviceType, contract);
                }
                catch (Exception)
                {
                    
                }

                return null;

              
            
            }

            public IEnumerable<object> GetServices(Type serviceType, string contract = null) {

                IEnumerable<object> retVal = null;

                retVal = Kernel.GetAll(serviceType, contract);
            
                if (retVal != null && retVal.Any())
                {
                    return retVal;
                }

                return _defaultResolver.GetServices(serviceType, contract);
         
            }

            public void Dispose() {
                Kernel.Dispose();
                _defaultResolver.Dispose();
            }
        }
    
}
