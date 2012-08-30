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
    public class DatabaseLoadViewModel
    {
        public IStorageFile Database { get; private set; }
        public IStorageFile KeyFile { get; private set; }

        public ICommand PickDatabase
        {
            get
            {
                return new DelegateCommand(ExecutePickDatabase);
            }
        }

        public ICommand PickKeyFile { get; set; }
        public ICommand LoadDatabase { get; set; }

        public DatabaseLoadViewModel()
        {
            
        }

        public async void ExecutePickDatabase()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".key");
            var file = await openPicker.PickSingleFileAsync();
        }
    }
}
