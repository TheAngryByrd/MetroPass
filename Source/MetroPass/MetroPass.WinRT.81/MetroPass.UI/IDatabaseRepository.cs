using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Newtonsoft.Json;

namespace MetroPass.UI
{
    public interface IDatabaseRepository
    {
        Task<IEnumerable<KeepassFilePair>> GetRecentFiles();
        Task SaveRecentFile(KeepassFilePair filePair);
        Task Delete(KeepassFilePair filePair);
        Task<KeepassFilePair> GetFilePairFromToken(KeepassFileTokenPair keepassFileTokenPair);
    }

    public class DatabaseRepository : IDatabaseRepository
    {
        private IStorageItemAccessList _recentFileList;
        private readonly IReactiveRoamingSettings<IEnumerable<KeepassFileTokenPair>> _reactiveRoamingSettings;

        public DatabaseRepository(IReactiveRoamingSettings<IEnumerable<KeepassFileTokenPair>> reactiveRoamingSettings = null,
            IStorageItemAccessList recentFileList = null)
        {
            _recentFileList = recentFileList ?? StorageApplicationPermissions.MostRecentlyUsedList;;
            _reactiveRoamingSettings = reactiveRoamingSettings ?? new ReactiveRoamingSettings<IEnumerable<KeepassFileTokenPair>>("recentFiles");
           
        }

        public async Task<IEnumerable<KeepassFilePair>> GetRecentFiles()
        {
            //HACK: Cleanup old stuff
            await ConvertOldTokens();

            var recentFilePairs = new List<KeepassFilePair>();

            var tokens = await GetFileTokenPairs();
            foreach (var keepassFileTokenPair in tokens)
            {
                recentFilePairs.Add(await GetFilePairFromToken(keepassFileTokenPair));
            }

            //Filter out file pairs without databases
            IEnumerable<KeepassFilePair> keepassFilePairs = recentFilePairs.Where(f => f.Database != null);
            return keepassFilePairs;
        }

        public async Task SaveRecentFile(KeepassFilePair filePair)
        {
            if (filePair.Database != null)
                _recentFileList.AddOrReplace(filePair.TokenPair.DatabaseFileToken, filePair.Database);
            if(filePair.KeeFile != null)
                _recentFileList.AddOrReplace(filePair.TokenPair.KeeFileToken, filePair.KeeFile);

            var tokens = await GetFileTokenPairs();
            var keepassFileTokenPairs = tokens.ToList();

            if (keepassFileTokenPairs.Any(t => t.DatabaseFileToken == filePair.TokenPair.DatabaseFileToken))
                return;


            keepassFileTokenPairs.Add(filePair.TokenPair);
            await SaveFileTokenPairs(keepassFileTokenPairs);
        }

        public async Task Delete(KeepassFilePair filePair)
        {
            if (filePair.Database != null && filePair.TokenPair.DatabaseFileToken != null)
                _recentFileList.Remove(filePair.TokenPair.DatabaseFileToken);
            if (filePair.KeeFile != null && filePair.TokenPair.KeeFileToken != null)
                _recentFileList.Remove(filePair.TokenPair.KeeFileToken);

            var tokens = await GetFileTokenPairs();
            var keepassFileTokenPairs = tokens.ToList();

            keepassFileTokenPairs =
                keepassFileTokenPairs.Where(
                    k =>
                        k.DatabaseFileToken != filePair.TokenPair.DatabaseFileToken &&
                        k.KeeFileToken != filePair.TokenPair.KeeFileToken).ToList();

            await SaveFileTokenPairs(keepassFileTokenPairs);
        }

        public async Task<KeepassFilePair> GetFilePairFromToken(KeepassFileTokenPair keepassFileTokenPair)
        {
            IStorageFile database = null;
            IStorageFile keeFile = null;

            if (!string.IsNullOrWhiteSpace(keepassFileTokenPair.DatabaseFileToken) && _recentFileList.ContainsItem(keepassFileTokenPair.DatabaseFileToken))
            {
                database = await _recentFileList.GetFileAsync(keepassFileTokenPair.DatabaseFileToken);
            }
            if (!string.IsNullOrWhiteSpace(keepassFileTokenPair.KeeFileToken) && _recentFileList.ContainsItem(keepassFileTokenPair.KeeFileToken))
            {
                keeFile = await _recentFileList.GetFileAsync(keepassFileTokenPair.KeeFileToken);
            }
            return new KeepassFilePair(database, keeFile, keepassFileTokenPair);
        }
        private async Task<IEnumerable<KeepassFileTokenPair>> GetFileTokenPairs()
        {
            IEnumerable<KeepassFileTokenPair> keepassFileTokenPairs = await _reactiveRoamingSettings.LoadSetting() ?? new List<KeepassFileTokenPair>();
            return keepassFileTokenPairs.Where(k => !string.IsNullOrWhiteSpace(k.DatabaseFileToken));
        }

        private async Task SaveFileTokenPairs(IEnumerable<KeepassFileTokenPair> pairs)
        {
            await _reactiveRoamingSettings.SaveSetting(pairs);
        }
        #region ConvertOldTokens

        private const string mostRecentDatabaseKey = "mostRecentDatabase";
        private const string mostRecentKeyFileKey = "mostRecentKeeFIle";
        /// <summary>
        /// Since I was foolish and didn't implement recent files as a list, I have to convert the old format to the new.  This should only happen once ever.
        /// </summary>
        /// <returns></returns>
        private async Task ConvertOldTokens()
        {
            var tokens = GetOldTokens();
            if (tokens.Any())
            {
                await SaveFileTokenPairs(tokens);
                ApplicationData.Current.RoamingSettings.Values.Remove(mostRecentDatabaseKey);
                ApplicationData.Current.RoamingSettings.Values.Remove(mostRecentKeyFileKey);
            }
        }
        private IEnumerable<KeepassFileTokenPair> GetOldTokens()
        {
            if (!ApplicationData.Current.RoamingSettings.Values.ContainsKey(mostRecentDatabaseKey))
            {
                return new List<KeepassFileTokenPair>();
            }

            string oldDatabaseToken = ApplicationData.Current.RoamingSettings.Values[mostRecentDatabaseKey].ToString();

            string oldKeeFileToken = null;

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(mostRecentKeyFileKey))
            {
                oldKeeFileToken = ApplicationData.Current.RoamingSettings.Values[mostRecentKeyFileKey].ToString();
            }
            return new[] { new KeepassFileTokenPair(oldDatabaseToken, oldKeeFileToken) };
        }
        #endregion
    }

    public struct KeepassFilePair
    {
        public KeepassFilePair(IStorageFile database, IStorageFile keefile, KeepassFileTokenPair tokenPair)
        {
            _database = database;
            _keeFile = keefile;
            _tokenPair = tokenPair;
        }

        private readonly IStorageFile _database;
        private readonly IStorageFile _keeFile;
        private readonly KeepassFileTokenPair _tokenPair;

        public IStorageFile Database
        {
            get { return _database; }
        }  
        public IStorageFile KeeFile
        {
            get { return _keeFile; }
        }
        public KeepassFileTokenPair TokenPair
        {
            get { return _tokenPair; }
        }
    }
    public struct KeepassFileTokenPair
    {
        public KeepassFileTokenPair(string databaseFileToken, string keeFileToken)
        {
            DatabaseFileToken = databaseFileToken;
            KeeFileToken = keeFileToken;
        }

        public string DatabaseFileToken;
        public string KeeFileToken;
    }

    public interface ISerializater<T>
    {
        Task<T> Deserialize(object item);
        Task<string> Serialize(T item);
    }

    public class JsonSerializer<T> : ISerializater<T>
    {
        public Task<T> Deserialize(object item)
        {
            try
            {
                return Task.Run(() => JsonConvert.DeserializeObject<T>(item.ToString(), new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }));
            }
            catch (Exception e)
            {
                return Task.FromResult(default(T));
            }

        }

        public Task<string> Serialize(T item)
        {
            return Task.Run(() => JsonConvert.SerializeObject(item));
        }
    }

    public interface IReactiveRoamingSettings<T> : IObservable<T>
    {
        Task SaveSetting(T item);
        Task<T> LoadSetting();
    }

    public class ReactiveRoamingSettings<T> : IReactiveRoamingSettings<T>
    {
        private readonly string _key;
        private readonly ISerializater<T> _serializer;
        private readonly IObservable<T> _observable;

        public ReactiveRoamingSettings(string key = "appSettings",
            ISerializater<T> serializer = null)
        {
            _key = key;
            _serializer = serializer ?? new JsonSerializer<T>();
            _observable = Observable.FromEventPattern(ApplicationData.Current, "DataChanged")
                .Select(_ => ApplicationData.Current.RoamingSettings.Values[_key])
                .SelectMany(o => Observable.FromAsync(() => _serializer.Deserialize(o)));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _observable.Subscribe(observer);
        }

        public async Task SaveSetting(T item)
        {
            var serializedItem = await _serializer.Serialize(item);
            ApplicationData.Current.RoamingSettings.Values[_key] = serializedItem;
        }

        public async Task<T> LoadSetting()
        {
            var container = ApplicationData.Current.RoamingSettings;

            if (!container.Values.ContainsKey(_key))
                return default(T);

            var serializedItem = container.Values[_key].ToString();
            var item = await _serializer.Deserialize(serializedItem);
            return item;
        }
    }

    public class HighPriorityRoamingSettings<T> : ReactiveRoamingSettings<T>
    {
        public HighPriorityRoamingSettings() : base("HighPriority") { }
    }
}
