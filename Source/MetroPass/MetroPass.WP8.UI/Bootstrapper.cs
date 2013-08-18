using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Ninject;

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
            string @namespace = "MetroPass.WP8.UI.ViewModels";

            var q = (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == @namespace
                    select t).ToList();

            q.ForEach(t =>_container.RegisterPerRequest(t,null,t));
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
        
    }

}
