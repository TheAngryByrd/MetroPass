using Framework;
using MetroPass.Core.Model.Keys;
using MetroPass.UI.Common;
using MetroPass.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MetroPass.UI.ViewModels
{
    public class LoadKdbPageViewModel : BindableBase
    {
        private Services.PageServices pageServices;
        private Services.NavigationService navigationService;

        public LoadKdbPageViewModel(Services.PageServices pageServices, Services.NavigationService navigationService)
        {
            // TODO: Complete member initialization
            this.pageServices = pageServices;
            this.navigationService = navigationService;
            
        }

        private IStorageFile _database;
        public IStorageFile Database
        {
            get
            {
                return _database;
            }
            set
            {
                SetProperty(ref _database, value);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty<string>(ref _password, value); }
        }

        private IStorageFile _keyFile;
        public IStorageFile KeyFile {

            get
            {
                return _keyFile;
            }
            set
            {
                SetProperty(ref _keyFile, value);
            }
        }
  
        private double _progress;    
        public double Progress
        {
            get { return _progress; }
            set { SetProperty<double>(ref _progress, value); }
        }
        public ICommand PickDatabase
        {
            get
            {
                return new DelegateCommand((Func<object, Task>)ExecutePickDatabase);
            }
        }

        public ICommand PickKeyFile
        {
            get
            {
                return new DelegateCommand((Func<object, Task>)ExecutePickKeyFile);
            }
        }

        public ICommand LoadDatabase
        {
            get
            {
                return new DelegateCommand((Func<object, Task>)ExecuteLoadBase);
            }
        }





        public async Task ExecutePickDatabase(object parameter)
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

        public async Task ExecutePickKeyFile(object obj)
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

        private async Task ExecuteLoadBase(object arg)
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

                navigationService.Navigate<EntryGroupListPage>(new EntryGroupListPageViewModel(PWDatabaseDataSource.Instance.Tree.Group));
            }
            catch (SecurityException se)
            {

                pageServices.Show(se.Message);
            }
          

      
        }
    }
}
