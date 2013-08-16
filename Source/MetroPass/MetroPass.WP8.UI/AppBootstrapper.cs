using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ninject;
using ReactiveUI;
using ReactiveUI.Mobile;

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

            public AppBootstrapper()
            {
                Router = new RoutingState();
                _ninjectResolver = new NinjectDependencyResolver(RxApp.DependencyResolver);
                RxApp.DependencyResolver = _ninjectResolver;

                //_ninjectResolver.Kernel.Bind<IViewFor<TestPage1ViewModel>>().To<TestPage1View>();

                _ninjectResolver.Kernel.Bind<IApplicationRootState>().ToConstant(this);

                _ninjectResolver.Kernel.Bind<IScreen>().ToConstant(this);
    

                //Router.Navigate.Execute(new TestPage1ViewModel(this));
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
                try
                {
                    return Kernel.GetAll(serviceType, contract);
                }
                catch (Exception)
                {

                }

                try
                {
                    return _defaultResolver.GetServices(serviceType, contract);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public void Dispose() {
                Kernel.Dispose();
                _defaultResolver.Dispose();
            }
        }
    
}
