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
        PhoneContainer container;
        IKernel kernel;

        protected override void Configure()
        {
            container = new PhoneContainer();
            kernel = new StandardKernel();
           

            container.RegisterPhoneServices(RootFrame);
            
            AddCustomConventions();
        }

        void AddCustomConventions()
        {
            string @namespace = "MetroPass.WP8.UI.ViewModels";

            var q = (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == @namespace
                    select t).ToList();

            q.ForEach(t =>container.RegisterPerRequest(t,null,t));
        }

        protected override object GetInstance(Type service, string key)
        {
            var result = container.GetInstance(service, key);
            if (result == null)
            {
                result =kernel.Get(service, key);
            }

            return result;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var result =  container.GetAllInstances(service);

            if (result == null || !result.Any())
            {
                result = kernel.GetAll(service);
            }

            return result;
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
            kernel.Inject(instance);
        }

        protected override Microsoft.Phone.Controls.PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new TransitionFrame();
        }
    }

}
