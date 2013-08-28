using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using Metropass.Core.PCL.Hashing;
using MetroPass.WP8.Infrastructure.Hashing;
using Microsoft.Phone.Controls;
using MetroPass.WP8.UI.DataModel;

namespace MetroPass.WP8.UI
{
    public class Bootstrapper : PhoneBootstrapper
    {
        PhoneContainer _container;

        protected override void Configure()
        {
            _container = new PhoneContainer();           

            _container.RegisterPhoneServices(RootFrame);
            
            AddCustomConventions();
        }

        void AddCustomConventions()
        {
            AddViewModels();
            ConfigureConvetions();
            _container.PerRequest<ICanSHA256Hash, SHA256HahserWP8>();
            _container.PerRequest<IDatabaseInfoRepository, DatabaseInfoRepository>();
        }
  
        private void AddViewModels() {
            string @namespace = "MetroPass.WP8.UI.ViewModels";

            var q = (from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && t.Namespace == @namespace select t).ToList();

            q.ForEach(t => _container.RegisterPerRequest(t, null, t));
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new TransitionFrame();
        }

        protected void ConfigureConvetions()
        {         
            ConventionManager.AddElementConvention<UIElement>(UIElement.VisibilityProperty, "Visibility", "VisibilityChanged");

            ConventionManager.AddElementConvention<BindableAppBarButton>(
            Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");

            var baseBindProperties = ViewModelBinder.BindProperties;
            ViewModelBinder.BindProperties =
                (frameWorkElements, viewModel) =>
                {
                    BindVisiblityProperties(frameWorkElements, viewModel);
                    return baseBindProperties(frameWorkElements, viewModel);
                };

            // Need to override BindActions as well, as it's called first and filters out anything it binds to before
            // BindProperties is called.
            var baseBindActions = ViewModelBinder.BindActions;
            ViewModelBinder.BindActions =
                (frameWorkElements, viewModel) =>
                {
                    BindVisiblityProperties(frameWorkElements, viewModel);
                    return baseBindActions(frameWorkElements, viewModel);
                };

        }


        void BindVisiblityProperties(IEnumerable<FrameworkElement> frameWorkElements, Type viewModel)
        {
            foreach (var frameworkElement in frameWorkElements)
            {
                var propertyName = frameworkElement.Name + "IsVisible";
                var property = viewModel.GetPropertyCaseInsensitive(propertyName);
                if (property != null)
                {
                    var convention = ConventionManager
                        .GetElementConvention(typeof(FrameworkElement));
                    ConventionManager.SetBindingWithoutBindingOverwrite(
                        viewModel,
                        propertyName,
                        property,
                        frameworkElement,
                        convention,
                        convention.GetBindableProperty(frameworkElement));
                }
            }
        }
    }

}
