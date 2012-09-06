using Caliburn.Micro;
using Framework;
using MetroPass.Core.Model.Keys;
using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using MetroPass.UI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MetroPass.UI.ViewModels
{
    public class LoadKdbViewModel : Screen
    {
        private IPageServices pageServices;
        private INavigationService navigationService;

        public LoadKdbViewModel(IPageServices pageServices, INavigationService navigationService)
        {
            this.pageServices = pageServices;
            this.navigationService = navigationService;
        }

        public void GoBack()
        {
            navigationService.GoBack();
        }

        public bool CanGoBack
        {
            get { return navigationService.CanGoBack; }
        }

        private IStorageFile _database;
        public IStorageFile Database
        {
            get { return _database; }
            set
            {
                _database = value;
                NotifyOfPropertyChange(() => Database);
                NotifyOfPropertyChange(() => CanOpenDatabase);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        private IStorageFile _keyFile;
        public IStorageFile KeyFile {

            get { return _keyFile; }
            set
            {
                _keyFile = value;
                NotifyOfPropertyChange(() => KeyFile);
            }
        }
  
        private double _progress;    
        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                NotifyOfPropertyChange(() => Progress);
            }
        }
        
        public async void PickDatabase()
        {
            if (await pageServices.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".kdbx");
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                    Database = file;
            }
        }

        public async void PickKeyFile()
        {
            if (await pageServices.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".key");
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                    KeyFile = file;
            }
        }
        public bool CanOpenDatabase
        {
            get { return Database != null; }
        }
        public async void OpenDatabase()
        {
            var userKeys = new List<IUserKey>();

            if (!string.IsNullOrEmpty(Password))
            {
                userKeys.Add(await KcpPassword.Create(Password));
            }
            if (KeyFile != null)
            {
                userKeys.Add(await KcpKeyFile.Create(KeyFile));
            }

            var progress = new Progress<double>(percent =>
            {
                percent = Math.Round(percent, 2);
                Progress = percent;
            });

            try
            {
                await PWDatabaseDataSource.Instance.LoadPwDatabase(Database, userKeys, progress);

                navigationService.NavigateToViewModel<EntryGroupListViewModel>(PWDatabaseDataSource.Instance.PwDatabase.Tree.Group);
            }
            catch (SecurityException se)
            {

                pageServices.Show(se.Message);
            }
        }

        public void OpenDemo()
        {
            PWDatabaseDataSource.Instance.SetupDemoData();
            navigationService.NavigateToViewModel<EntryGroupListViewModel>(PWDatabaseDataSource.Instance.PwDatabase.Tree.Group);
        }
    }
}
