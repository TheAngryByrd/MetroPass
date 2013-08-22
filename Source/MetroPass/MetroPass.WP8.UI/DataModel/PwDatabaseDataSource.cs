using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MetroPass.WP8.Infrastructure.Compression;
using MetroPass.WP8.Infrastructure.Cryptography;
using MetroPass.WP8.Infrastructure.Hashing;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using Windows.Storage;
using PCLStorage;

namespace MetroPass.WP8.UI.DataModel
{
    public sealed class PWDatabaseDataSource
    {
        private static PWDatabaseDataSource instance = new PWDatabaseDataSource();
        private PWDatabaseDataSource()
        {

        }
        public static PWDatabaseDataSource Instance { get { return instance; } }

        public IStorageFile StorageFile;
        private PwDatabase _pwDatabase;
        public PwDatabase PwDatabase
        {
            get { return _pwDatabase; }
            set { _pwDatabase = value; }
        }

        public async Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            StorageFile = pwDatabaseFile;
            var factory = new KdbReaderFactory(
                new ManagedCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                new MultithreadedManagedCrypto(),
                new SHA256HahserWP8(),
                new GZipFactoryWP8());

            var file = await pwDatabaseFile.OpenAsync(FileAccessMode.Read);

            Stream kdbDataReader = file.AsStream();

            this.PwDatabase = await factory.LoadAsync(kdbDataReader, userKeys, percentComplete);
        }

        public async Task SavePwDatabase()
        {
            var factory = new KdbWriterFactory(new ManagedCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                      new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB),
                      new SHA256HahserWP8(),
                      new GZipFactoryWP8());

            var writer = factory.CreateWriter(PwDatabase.Tree);            
            
            await writer.Write(PwDatabase, new WP8File(StorageFile));

        }
    } 
}
