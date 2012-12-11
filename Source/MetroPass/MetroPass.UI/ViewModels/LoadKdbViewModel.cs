using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using Caliburn.Micro;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Keys;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.Views;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace MetroPass.UI.ViewModels
{
    public class LoadKdbViewModel : BaseScreen
    {
        private readonly IPageServices _pageServices;
        private readonly INavigationService _navigationService;

 

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
              
                NotifyOfPropertyChange(() => Path);
            }
        }
        private string _parameter;
        public string Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;

                NotifyOfPropertyChange(() => Parameter);
            }
        }


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
                    StoreMostRecentFile(mostRecentDatabaseKey, file);

                    var view = View as ILoadKdbView;
                    if (view != null)
                    {
                        view.FocusPassword();
                    }
                }
            }
        }
  
        private const string mostRecentDatabaseKey = "mostRecentDatabase";
        private const string mostRecentKeeFileKey = "mostRecentKeeFIle";

        private void StoreMostRecentFile(string keyValue, StorageFile file)
        {
            var storageList =  StorageApplicationPermissions.MostRecentlyUsedList;
            if(storageList.ContainsItem(keyValue))
            {
                storageList.Remove(keyValue);
            }
       
            var mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
            ApplicationData.Current.RoamingSettings.Values[keyValue] = mruToken;
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
                {
                    KeyFile = file;
                    StoreMostRecentFile(mostRecentKeeFileKey, file);
                }
                   
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

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            var storageList = StorageApplicationPermissions.MostRecentlyUsedList;
            var roamingSettings = ApplicationData.Current.RoamingSettings;

            if(roamingSettings.Values.ContainsKey(mostRecentKeeFileKey))
            {
                var keeFileToken = roamingSettings.Values[mostRecentKeeFileKey].ToString();
                if (storageList.ContainsItem(keeFileToken))
                {
                    KeyFile = await storageList.GetFileAsync(keeFileToken);
                } 
            }

            if (PWDatabaseDataSource.Instance.StorageFile != null)
            {
                Database = PWDatabaseDataSource.Instance.StorageFile;
                PWDatabaseDataSource.Instance.StorageFile = null;
            }
            else if (roamingSettings.Values.ContainsKey(mostRecentDatabaseKey))
            {
                var databaseToken =roamingSettings.Values[mostRecentDatabaseKey].ToString();
                if (storageList.ContainsItem(databaseToken))
                {
                    Database = await storageList.GetFileAsync(databaseToken);
                }              
            }
            else
            {
                PickDatabase();
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
                var encodedUUID = WebUtility.UrlEncode(PWDatabaseDataSource.Instance.PwDatabase.Tree.Group.UUID);

                _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
            }
            catch (SecurityException se)
            {
                OpeningDatabase = false;
                _pageServices.Show(se.Message);
            }
        }
    }
}