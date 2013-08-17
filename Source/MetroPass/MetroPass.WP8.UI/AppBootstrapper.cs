using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Navigation;
using MetroPass.WP8.UI.ViewModels;
using MetroPass.WP8.UI.Views;
using Ninject;
using ReactiveUI;
using ReactiveUI.Mobile;

public static class PropertySupport
{
    /// <summary>
    /// Extracts the property name from a property expression.
    /// </summary>
    /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
    /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
    /// <returns>The name of the property.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
    ///     Not a <see cref="MemberExpression"/><br/>
    ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
    ///     Or, the property is static.
    /// </exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression == null)
        {
            throw new ArgumentNullException("propertyExpression");
        }

        var memberExpression = propertyExpression.Body as MemberExpression;
        if (memberExpression == null)
        {
            throw new ArgumentException();
        }

        var property = memberExpression.Member as PropertyInfo;
        if (property == null)
        {
            throw new ArgumentException();
        }

        var getMethod = property.GetGetMethod(true);
        if (getMethod.IsStatic)
        {
            throw new ArgumentException();
        }

        return memberExpression.Member.Name;
    }
}

public static class ReactiveUINavigateExtentions
{
    public static void Navigate<T>(this INavigateCommand command)
    {
        var VM = RxApp.DependencyResolver.GetService<T>();
        command.Execute(VM);
    }

    public static void Navigate<T>(this INavigateCommand command, Expression<Func<T>> propertyExpression, object value)
    {
        var VM = RxApp.DependencyResolver.GetService<T>();
        PropertySupport.ExtractPropertyName(propertyExpression);

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
