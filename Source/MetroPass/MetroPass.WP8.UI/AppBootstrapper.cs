using System;
using System.Runtime.Serialization;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels;
using MetroPass.WP8.UI.Views;
using ReactiveUI;
using ReactiveUI.Mobile;

namespace MetroPass.WP8.UI
{
    [DataContract]
        public class AppBootstrapper : ReactiveObject, IApplicationRootState
    {
            [DataMember]
            IRoutingState _Router;

            public IRoutingState Router
            {
                get { return _Router; }
                set { _Router = value; } // XXX: This is dumb.
            }

            [IgnoreDataMember]
            private readonly NinjectDependencyResolver _ninjectResolver;

        public AppBootstrapper(NinjectDependencyResolver resolver)
        {
                _ninjectResolver = resolver;
                Router = new RoutingState();

            SetupBindings();

            Router.Navigate.Navigate<DatabaseListViewModel, Guid>(b => b.RandomGuid, Guid.NewGuid());
        }
  
        private void SetupBindings() {
            _ninjectResolver.Kernel.Bind<IViewFor<DatabaseListViewModel>>().To<DatabaseListView>();
            _ninjectResolver.Kernel.Bind<IViewFor<SkydriveAccessViewModel>>().To<SkydriveAccessView>();         

            _ninjectResolver.Kernel.Bind<IApplicationRootState>().ToConstant(this);

            _ninjectResolver.Kernel.Bind<IScreen>().ToConstant(this);
        }
    }
}
