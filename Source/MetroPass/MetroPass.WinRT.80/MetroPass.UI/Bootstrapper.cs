﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.WinRT.Infrastructure.PasswordGeneration;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.PasswordGeneration;
using Ninject;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI
{
    public class Bootstrapper
    {
        private NinjectContainer _ninjectContainer;

        public Bootstrapper()
        {
            _ninjectContainer = new NinjectContainer();
        }

        public void Configure()
        {
            _ninjectContainer.RegisterWinRTServices();

            _ninjectContainer.Kernel.Bind<IPageServices>().To<PageServices>();
            _ninjectContainer.Kernel.Bind<IPasswordGenerator>().To<PasswordGeneratorRT>();

            _ninjectContainer.Kernel.Bind<ILockingService>().To<LockingService>();
            _ninjectContainer.Kernel.Bind<IClipboard>().To<MetroClipboard>();
            _ninjectContainer.Kernel.Bind<IKdbTree>().ToMethod(c => PWDatabaseDataSource.Instance.PwDatabase.Tree);
        }

        public object GetInstance(Type service, string key)
        {
            var instance = _ninjectContainer.Kernel.Get(service, key);
            if (instance is IHandle)
            {
                _ninjectContainer.Kernel.Get<IEventAggregator>().Subscribe(instance);
            }
            return instance;
        }

         public  IEnumerable<object> GetAllInstances(Type service)
         {
             return _ninjectContainer.Kernel.GetAll(service);
         }

         public void BuildUp(object instance)
         {
             _ninjectContainer.Kernel.Inject(instance);
         }

         public void PrepareViewFirst(Frame rootFrame)
         {
             _ninjectContainer.RegisterNavigationService(rootFrame);
         }
    }
}
