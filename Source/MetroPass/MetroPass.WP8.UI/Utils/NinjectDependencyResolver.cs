using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using ReactiveUI;

namespace MetroPass.WP8.UI.Utils
{
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