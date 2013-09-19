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

        private readonly IPWDatabaseDataSource _dataSource;

        public LockingService(INavigationService navigationService,
            IPWDatabaseDataSource dataSource)
        {
            _dataSource = dataSource;
            this.navigationService = navigationService;
        }

        public void Lock()
        {
            _dataSource.PwDatabase = null;
            _dataSource.StorageFile = null;
            navigationService.UriFor<StartPageViewModel>().Navigate();
        }

        public void OnResumeLock(DateTime suspendedDate)
        {
            TimeSpan ts = DateTime.Now.Subtract(suspendedDate);

            if (SettingsModel.Instance.LockDatabaseAfterInactivityEnabled)
            {
                if (ts.Seconds > SettingsModel.Instance.MinutesToLockDatabase)
                {
                    Lock();
                }
            }            
        }
    }
}
