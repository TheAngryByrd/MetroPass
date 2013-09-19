using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using System.IO;
using Metropass.Core.PCL.Hashing;
using Metropass.Core.PCL.Compression;

namespace Metropass.Core.PCL.Model.Kdb4.Reader
{
    public class KdbReaderFactory
    {
        private readonly IEncryptionEngine _databaseDecryptor;
        private readonly IKeyTransformer _keyTransformer;
        private readonly ICanSHA256Hash _hasher;
        private readonly IGZipStreamFactory _gzipFactory;

        public KdbReaderFactory(IEncryptionEngine databaseDecryptor,
            IKeyTransformer keyTransformer, 
            ICanSHA256Hash hasher, 
            IGZipStreamFactory gzipFactory)
        {
            _gzipFactory = gzipFactory;
            _hasher = hasher;
            _keyTransformer = keyTransformer;
            _databaseDecryptor = databaseDecryptor;
        }

        public Task<PwDatabase> LoadAsync(Stream database, List<IUserKey> userKeys)
        {
            return LoadAsync(database, userKeys, new NullableProgress<double>());
        }
 
        public async Task<PwDatabase> LoadAsync(Stream kdbDatabase, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
        
            var versionInfo = ReadVersionInfo(kdbDatabase);
            IKdbReader reader = null;
            var compositeKey = new CompositeKey(userKeys, percentComplete);
            var pwDatabase = new PwDatabase(compositeKey);
          
            if (IsKdb4(versionInfo))
            {
                  var kdb4File = new Kdb4File(pwDatabase);

                  reader = new Kdb4Reader(kdb4File,
                      _databaseDecryptor,
                      _keyTransformer,
                      _hasher,
                      _gzipFactory);     
            }
            else
            {
                throw new FormatException();
            }
            pwDatabase.Tree = await reader.Load(kdbDatabase);
            return pwDatabase;

        }

        private static bool IsKdb4(VersionInfo versionInfo)
        {
            return versionInfo.FileSignature1 == KdbConstants.FileSignature1 
                && versionInfo.FileSignature2 == KdbConstants.FileSignature2;
        }

        public VersionInfo ReadVersionInfo(Stream kdbReader)
        {
            var versionInfo = new VersionInfo();
            var readerBytes = new byte[4];
            
            

            kdbReader.ReadBytes(readerBytes);
            versionInfo.FileSignature1 = BitConverter.ToUInt32(readerBytes, 0);
            kdbReader.ReadBytes(readerBytes);
            versionInfo.FileSignature2 = BitConverter.ToUInt32(readerBytes, 0);
            kdbReader.ReadBytes(readerBytes);
            versionInfo.Version = BitConverter.ToUInt32(readerBytes, 0);

            return versionInfo;
        }
    }

    public class VersionInfo
    {
        public UInt32 FileSignature1 {get;set;}
        public UInt32 FileSignature2 {get;set;}
        public UInt32 Version {get;set;}

    }
}
