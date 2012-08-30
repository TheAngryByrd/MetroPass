using MetroPass.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MetroPass.UI.ViewModels
{
    public class DatabaseLoadViewModel : BindableBase
    {
        public IStorageFile Database { get; private set; }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty<string>(ref _password, value); }
        }

        public IStorageFile KeyFile { get; private set; }

        

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
                return new DelegateCommand((Func<object,Task>)ExecutePickKeyFile);
            }
        }

        public ICommand LoadDatabase
        {
            get
            {
                return new DelegateCommand((Func<object, Task>)ExecuteLoadBase);
            }
        }

        private Task ExecuteLoadBase(object arg)
        {
            throw new NotImplementedException();
        }

        public DatabaseLoadViewModel()
        {
            
        }

        public async Task ExecutePickDatabase(object parameter)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".kdbx");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
                Database = file;
        }

        public async Task ExecutePickKeyFile(object obj)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".kdbx");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
                KeyFile = file;
        }
    }
}
