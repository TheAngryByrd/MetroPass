﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.Views;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using PCLStorage;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace MetroPass.UI.ViewModels
{
    public class LoadKdbViewModel : BaseScreen
    {
        private readonly IPageServices _pageServices;
        private readonly INavigationService _navigationService;

        private readonly IPWDatabaseDataSource _dataSource;

        public LoadKdbViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPageServices pageServices,
            IPWDatabaseDataSource dataSource)
            : base(navigationService, eventAggregator, pageServices)
        {
            _dataSource = dataSource;
            _pageServices = pageServices;
            _navigationService = navigationService;
        }

        //This parameter should only be set when the user is trying to search from the Search Charm but MetroPass is not running
        //(or the database has been locked by the timer)
        public string Parameter
        {
            get { return _searchText; }
            set { SearchText = value; }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        public bool ShouldRedirectToSearch
        {
            get { return !String.IsNullOrWhiteSpace(SearchText); }
        }

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

        private IStorageFile _database;
        public IStorageFile Database
        {
            get { return _database; }
            set
            {
                _database = value;
                ResaveRecentFile(mostRecentDatabaseKey, _database);
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
                ResaveRecentFile(mostRecentKeyFileKey, _keyFile);
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
                openPicker.FileTypeFilter.Add("*");
                       
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    Database = file;
                   
                }
            }
        }

        public void ClearFiles()
        {
            Database = null;
            Password = string.Empty;
            KeyFile = null;
            SearchText = string.Empty;
            SetState("Normal");
        }
  
        private void FocuxPassword()
        {
            var view = View as ILoadKdbView;
            if (view != null)
            {
                view.FocusPassword();
            }
        }
  
        public async void PickKeyFile()
        {
            if (await _pageServices.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add("*");
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    KeyFile = file;
                
                    FocuxPassword();
                }
            }
        }

        private const string mostRecentDatabaseKey = "mostRecentDatabase";
        private const string mostRecentKeyFileKey = "mostRecentKeeFIle";

        private void ResaveRecentFile(string keyValue, IStorageFile file)
        {
            var storageList = StorageApplicationPermissions.MostRecentlyUsedList;
            string token = Guid.NewGuid().ToString();

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(keyValue))
            {
                token = ApplicationData.Current.RoamingSettings.Values[keyValue].ToString();
            }      

            if (file != null)
            {
                StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(token, file);
                ApplicationData.Current.RoamingSettings.Values[keyValue] = token;
            }
            else if(file == null && StorageApplicationPermissions.MostRecentlyUsedList.ContainsItem(token))
            {
                StorageApplicationPermissions.MostRecentlyUsedList.Remove(token);
            }
        }
  

        public bool CanOpenDatabase
        {
            get { return Database != null && !OpeningDatabase; }
        }

        private bool _canPickDatabase = true;
        public bool CanPickDatabase
        {
            get { return _canPickDatabase && !OpeningDatabase; }
            set
            {
                _canPickDatabase = value;
                NotifyOfPropertyChange(() => CanPickDatabase);
                NotifyOfPropertyChange(() => CanClearFiles);
            }
        }

        private bool _canPickKeyFile = true;
        public bool CanPickKeyFile
        {
            get { return _canPickKeyFile && !OpeningDatabase; }
            set
            {
                _canPickKeyFile = value;
                NotifyOfPropertyChange(() => CanPickKeyFile);
                NotifyOfPropertyChange(() => CanClearFiles);
            }
        }

        private bool _openingDatabase = false;
        private bool OpeningDatabase
        {
            get { return _openingDatabase; }
            set
            {
                _openingDatabase = value;
                NotifyOfPropertyChange(() => CanPickKeyFile);
                NotifyOfPropertyChange(() => CanPickDatabase);

                NotifyOfPropertyChange(() => CanOpenDatabase);
                NotifyOfPropertyChange(() => CanClearFiles);
                NotifyOfPropertyChange(() => OpeningDatabase);
            }
        }

        private bool _isIndeterminateProgressBarVisible;
        public bool IsIndeterminateProgressBarVisible
        {
            get { return _isIndeterminateProgressBarVisible; }
            set
            {
                _isIndeterminateProgressBarVisible = value;
                NotifyOfPropertyChange(() => IsIndeterminateProgressBarVisible);
            }
        }

        public bool CanClearFiles 
        {
            get { return CanPickDatabase && CanPickKeyFile && !OpeningDatabase; }
        }

        protected internal async override Task OnViewLoaded(object view)
        {
            await base.OnViewLoaded(view);

            if (ShouldRedirectToSearch) {
                SetState("Searching");
            }

            var storageList = StorageApplicationPermissions.MostRecentlyUsedList;
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            var taskList = new List<Task>();
            taskList.Add(TryLoadLastKeefile(roamingSettings, storageList));
            taskList.Add(TryLoadLastDatabase(roamingSettings, storageList));
            Func<Task> loadLastTasks = async () => await Task.WhenAll(taskList);
            await DisableEnableBlock(loadLastTasks);
        }

        private async Task DisableEnableBlock(Func<Task> loadLastTasks)
        {
            IsIndeterminateProgressBarVisible = true;
            EnablePickers(false);
            await loadLastTasks();
            EnablePickers(true);
            IsIndeterminateProgressBarVisible = false;
            FocuxPassword();
        }

        private void EnablePickers(bool isEnabled)
        {
            CanPickDatabase = isEnabled;
            CanPickKeyFile = isEnabled;
        }

        private const string FileNotFoundMessage = "MetroPass couldn't find your {0}.  It may have moved, been renamed or if it's on skydrive, check your internet connection.";

        private async Task TryLoadLastDatabase(ApplicationDataContainer roamingSettings, StorageItemMostRecentlyUsedList storageList)
        {
            var pickDatabase = true;

            if (_dataSource.StorageFile != null)
            {
                Database = _dataSource.StorageFile;
                _dataSource.StorageFile = null;
    
                pickDatabase = false;
            }
            else if (roamingSettings.Values.ContainsKey(mostRecentDatabaseKey))
            {
                try
                {
                    var databaseToken = roamingSettings.Values[mostRecentDatabaseKey].ToString();
                    if (storageList.ContainsItem(databaseToken))
                    {
                        Database = await storageList.GetFileAsync(databaseToken);
                        pickDatabase = false;
             
                    }
                }
                catch (FileNotFoundException fnf)
                {
                    _pageServices.Toast(string.Format(FileNotFoundMessage, "database"));
                }
               catch(Exception e)
                {
                    
                }
            }
            if (pickDatabase)
            {
                PickDatabase();
            }
        }

        private async Task TryLoadLastKeefile(ApplicationDataContainer roamingSettings, StorageItemMostRecentlyUsedList storageList)
        {
            try
            {
                if (roamingSettings.Values.ContainsKey(mostRecentKeyFileKey))
                {
                    var keeFileToken = roamingSettings.Values[mostRecentKeyFileKey].ToString();
                    if (storageList.ContainsItem(keeFileToken))
                    {
                        KeyFile = await storageList.GetFileAsync(keeFileToken);
                    }
                }
            }
            catch (FileNotFoundException fnf)
            {
                _pageServices.Toast(string.Format(FileNotFoundMessage, "keyfile"));
            }
            catch (Exception e)
            {
                
            }
        }

        public async void OpenDatabase()
        {
            OpeningDatabase = true;
            var userKeys = new List<IUserKey>();
            var sHA256HasherRT = new SHA256HasherRT();
            if (!string.IsNullOrEmpty(Password))
            {
         
                userKeys.Add(await KcpPassword.Create(Password, sHA256HasherRT));
            }
            if (KeyFile != null)
            {
                userKeys.Add(await KcpKeyFile.Create(new WinRTFile(KeyFile), sHA256HasherRT));
            }
           
            var progress = new Progress<double>(percent =>
            {
                percent = Math.Round(percent, 2);
                Progress = percent;
            });

            try
            {
                await _dataSource.LoadPwDatabase(Database, userKeys, progress);
                OpeningDatabase = false;
                var encodedUUID = _dataSource.PwDatabase.Tree.Group.UUID;

                if (ShouldRedirectToSearch)
                {
                    _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
                    _navigationService.UriFor<SearchResultsViewModel>().WithParam(vm => vm.QueryText, SearchText).Navigate();
                }
                else
                {
                    _navigationService.UriFor<EntryGroupListViewModel>().WithParam(vm => vm.GroupId, encodedUUID).Navigate();
                }
            }
            catch (Exception se)
            {
                OpeningDatabase = false;
                Progress = 0;
                _pageServices.Toast(se.Message);
            }
        }
    }
}