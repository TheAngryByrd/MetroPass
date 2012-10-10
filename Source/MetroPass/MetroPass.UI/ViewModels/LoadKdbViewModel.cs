using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Caliburn.Micro;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Keys;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.Views;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MetroPass.UI.ViewModels
{
    public class LoadKdbViewModel : BaseScreen
    {
        private readonly IPageServices _pageServices;
        private readonly INavigationService _navigationService;

        public LoadKdbViewModel(IPageServices pageServices, INavigationService navigationService) : base(navigationService)
        {
            _pageServices = pageServices;
            _navigationService = navigationService;
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
            if (await _pageServices.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".kdbx");
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    Database = file;
                    var view = View as ILoadKdbView;
                    if (view != null)
                    {
                        view.FocusPassword();
                    }
                }
            }
        }

        public async void PickKeyFile()
        {
            if (await _pageServices.EnsureUnsnapped())
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
            get { return Database != null && !OpeningDatabase; }
        }

        private bool _openingDatabase = false;
        private bool OpeningDatabase
        {
            get { return _openingDatabase; }
            set
            {
                _openingDatabase = value;
                NotifyOfPropertyChange(() => CanOpenDatabase);
            }
        }

        public async void OpenDatabase()
        {
            OpeningDatabase = true;
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
                OpeningDatabase = false;
                _navigationService.NavigateToViewModel<EntryGroupListViewModel, PwGroup>(PWDatabaseDataSource.Instance.PwDatabase.Tree.Group, vm => vm.Root);
            }
            catch (SecurityException se)
            {
                OpeningDatabase = false;
                _pageServices.Show(se.Message);
            }
        }
    }
}