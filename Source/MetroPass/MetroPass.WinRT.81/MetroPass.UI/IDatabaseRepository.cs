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

    public class AsyncLazy<T> : Lazy<Task<T>> 
{ 
    public AsyncLazy(Func<T> valueFactory) : 
        base(() => Task.Factory.StartNew(valueFactory)) { }
    public AsyncLazy(Func<Task<T>> taskFactory) : 
        base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { } 
}
    public interface IDatabaseRepository
    {
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

        private async Task<KeepassFilePair> GetFilePairFromToken(KeepassFileTokenPair keepassFileTokenPair)
        {
            var keepassFilePair = new KeepassFilePair();
            if (_recentFileList.ContainsItem(keepassFileTokenPair.DatabaseFileToken))
            {
                keepassFilePair.Database = await _recentFileList.GetFileAsync(keepassFileTokenPair.DatabaseFileToken);
            }
            if (_recentFileList.ContainsItem(keepassFileTokenPair.KeeFileToken))
            {
                keepassFilePair.KeeFile = await _recentFileList.GetFileAsync(keepassFileTokenPair.KeeFileToken);
            }
            return keepassFilePair;
        }
        private async Task<IEnumerable<KeepassFileTokenPair>> GetFileTokenPairs()
        {
            return await _reactiveRoamingSettings.LoadSetting() ?? new List<KeepassFileTokenPair>();
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
            var oldToken = new KeepassFileTokenPair();
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(mostRecentDatabaseKey))
            {
                oldToken.DatabaseFileToken = ApplicationData.Current.RoamingSettings.Values[mostRecentDatabaseKey].ToString();
            }
            else
            {
                return new List<KeepassFileTokenPair>();
            }

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(mostRecentKeyFileKey))
            {
                oldToken.KeeFileToken = ApplicationData.Current.RoamingSettings.Values[mostRecentKeyFileKey].ToString();
            }
            return new[] { oldToken };
        }
        #endregion
    }

    public struct KeepassFilePair
    {
        public IStorageFile Database { get; set; }
        public IStorageFile KeeFile { get; set; }
    }
    public struct KeepassFileTokenPair
    {
        public string DatabaseFileToken { get; set; }
        public string KeeFileToken { get; set; }
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
                return Task.Run(() => JsonConvert.DeserializeObject<T>(item.ToString()));
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

            var serializedItem = container.Values[_key];
            var item = await _serializer.Deserialize(serializedItem);
            return item;
        }
    }

    public class HighPriorityRoamingSettings<T> : ReactiveRoamingSettings<T>
    {
        public HighPriorityRoamingSettings() : base("HighPriority") { }
    }
}
