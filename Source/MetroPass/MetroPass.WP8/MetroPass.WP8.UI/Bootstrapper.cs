using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using MetroPass.UI.DataModel;
using MetroPass.WP8.Infrastructure.Compression;
using MetroPass.WP8.UI.Services.Cloud;
using MetroPass.WP8.UI.Services.UI;
using MetroPass.WP8.UI.Utils;
using Metropass.Core.PCL.Compression;
using Metropass.Core.PCL.Hashing;
using MetroPass.WP8.Infrastructure.Hashing;
using Microsoft.Phone.Controls;
using MetroPass.WP8.UI.DataModel;
using Metropass.Core.PCL.Encryption;
using MetroPass.WP8.Infrastructure.Cryptography;

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

            RootFrame.Navigated += RootFrame_Navigated;
        }

        void RootFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Reset)
            {
                RootFrame.Navigating += RootFrame_Navigating;
            }
        }

        private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New  && e.Uri.OriginalString == "/Views/DatabaseListView.xaml")
            {
                e.Cancel = true;
                RootFrame.Navigating -= RootFrame_Navigating;
            }
        }

        void AddCustomConventions()
        {
            AddViewModels();
            AddView();
            ConfigureConvetions();
            _container.PerRequest<ICanSHA256Hash, SHA256HasherWP8>();
            _container.PerRequest<IDatabaseInfoRepository, DatabaseInfoRepository>();
            _container.PerRequest<IDialogService, DialogService>();
            _container.PerRequest<ICloudProviderFactory, CloudProviderFactory>();
            _container.PerRequest<IEncryptionEngine, ManagedCrypto>();
            _container.PerRequest<IKeyTransformer, MultiThreadedBouncyCastleCrypto>();
            _container.PerRequest<IGZipStreamFactory, GZipFactoryWP8>();
            _container.Singleton<IPWDatabaseDataSource, PWDatabaseDataSource>();
            _container.Singleton<ICache, Cache>();
        }
  
        private void AddViewModels() {
            string @namespace = "MetroPass.WP8.UI.ViewModels";

            var q = (from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && t.Namespace == @namespace select t).ToList();

            q.ForEach(t => _container.RegisterPerRequest(t, null, t));
        } 
        private void AddView() {
            string @namespace = "MetroPass.WP8.UI.Views";

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
