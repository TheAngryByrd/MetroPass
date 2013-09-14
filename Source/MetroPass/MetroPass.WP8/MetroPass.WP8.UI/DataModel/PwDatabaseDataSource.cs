using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Metropass.Core.PCL.Compression;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using Windows.Storage;
using PCLStorage;

namespace MetroPass.UI.DataModel
{
    public sealed class PWDatabaseDataSource : IPWDatabaseDataSource
    {       
        private readonly IEncryptionEngine _encryptionEngine;
        private readonly IKeyTransformer _keyTransformer;
        private readonly IGZipStreamFactory _gzipStreamFactory;
        private readonly ICanSHA256Hash _hasher;

        public PWDatabaseDataSource(IEncryptionEngine encryptionEngine,
            IKeyTransformer keyTransformer,
            IGZipStreamFactory gzipStreamFactory,
            ICanSHA256Hash hasher)
        {
            _hasher = hasher;
            _gzipStreamFactory = gzipStreamFactory;
            _keyTransformer = keyTransformer;
            _encryptionEngine = encryptionEngine;
        }

        public IStorageFile StorageFile { get; set; }

        private PwDatabase _pwDatabase;
        public PwDatabase PwDatabase
        {
            get { return _pwDatabase; }
            set { _pwDatabase = value; }
        }

        public Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys)
        {
            return LoadPwDatabase(pwDatabaseFile, userKeys, new NullableProgress<double>());
        }

        public async Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            StorageFile = pwDatabaseFile;
            var factory = new KdbReaderFactory(_encryptionEngine,
                      _keyTransformer,
                      _hasher,
                      _gzipStreamFactory);

            var file = await pwDatabaseFile.OpenAsync(FileAccessMode.Read);

            Stream kdbDataReader = file.AsStream();

            this.PwDatabase = await factory.LoadAsync(kdbDataReader, userKeys, percentComplete);
        }

        public async Task SavePwDatabase()
        {
            var factory = new KdbWriterFactory(_encryptionEngine,
                      _keyTransformer,
                      _hasher,
                      _gzipStreamFactory);

            var writer = factory.CreateWriter(PwDatabase.Tree);  
            
#if NETFX_CORE
            IFile file = new WinRTFile(StorageFile);
#elif WP8
            IFile file = new WP8File(StorageFile);
#endif
            await writer.Write(PwDatabase, file);
        }
    } 
}
