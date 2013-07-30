using Framework;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Model.Keys;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Metropass.Core.PCL;

namespace MetroPass.Core.Services
{
    public class KdbReaderFactory
    {
        public Task<PwDatabase> LoadAsync(IStorageFile database, List<IUserKey> userKeys)
        {
            return LoadAsync(database, userKeys, new NullableProgress<double>());
        }
 
        public async Task<PwDatabase> LoadAsync(IStorageFile kdbDatabase, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            
            var kdbDataReader =  DataReader.FromBuffer(await FileIO.ReadBufferAsync(kdbDatabase));
            var versionInfo = ReadVersionInfo(kdbDataReader);
            IKdbReader reader = null;
            var compositeKey = new CompositeKey(userKeys, percentComplete);
            var pwDatabase = new PwDatabase(compositeKey);
          
            if (IsKdb4(versionInfo))
            {
                  var kdb4File = new Kdb4File(pwDatabase);

                reader = new Kdb4Reader(kdb4File);      
            }
            else
            {
                throw new FormatException();
            }
            pwDatabase.Tree = await reader.Load(kdbDataReader);
            return pwDatabase;

        }

        private static bool IsKdb4(VersionInfo versionInfo)
        {
            return versionInfo.FileSignature1 == KdbConstants.FileSignature1 && versionInfo.FileSignature2 == KdbConstants.FileSignature2;
        }

        public VersionInfo ReadVersionInfo(IDataReader kdbReader)
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
