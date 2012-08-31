using MetroPass.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Security;
using MetroPassLib;
using MetroPassLib.Keys;
using Windows.Storage.Streams;

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

        private IDialogService _dialogService { get; set; }

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


        public DatabaseLoadViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
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
            openPicker.FileTypeFilter.Add(".key");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
                KeyFile = file;
        }

        private async Task ExecuteLoadBase(object arg)
        {
            var bufferedData = await FileIO.ReadBufferAsync(Database);
            PwDatabase pwDatabase = new PwDatabase();
            var composite = new CompositeKey();
            if (!string.IsNullOrEmpty(Password))
            {
                composite.UserKeys.Add(await KcpPassword.Create(Password));
            }
            if (KeyFile != null)
            {
                composite.UserKeys.Add(await KcpKeyFile.Create(KeyFile));
            }

            pwDatabase.MasterKey = composite;
            Kdb4File kdb4 = new Kdb4File(pwDatabase);
            try
            {

                var tree = await kdb4.Load(DataReader.FromBuffer(bufferedData), Kdb4Format.Default);
               await _dialogService.Show("Decrypted database");
            }
            catch(Exception e)
            {
                 _dialogService.Show("Decryption failed");
            }
        }

    }
}
