using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.ViewModels;

namespace MetroPass.UI.Services
{
    public class LockingService : ILockingService
    {
        private readonly INavigationService navigationService;

        public LockingService(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void Lock()
        {
            navigationService.UriFor<StartPageViewModel>().Navigate();
        }

        public void OnResumeLock(DateTime suspendedDate)
        {
            TimeSpan ts = DateTime.Now.Subtract(suspendedDate);

            if (SettingsModel.MinutesToLockDatabase > 0)
            {
                if (ts.Seconds > SettingsModel.MinutesToLockDatabase)
                {
                    Lock();
                }
            }            
        }
    }
}
