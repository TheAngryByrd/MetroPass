using Framework;
using MetroPass.Core.Helpers;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Model.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MetroPass.Core.Services
{
    public class KdbReaderFactory
    {
        /// <summary>
        /// File identifier, first 32-bit value.
        /// </summary>
        private const uint FileSignature1 = 0x9AA2D903;

        /// <summary>
        /// File identifier, second 32-bit value.
        /// </summary>
        private const uint FileSignature2 = 0xB54BFB67;

        // KeePass 1.x signature
        private const uint FileSignatureOld1 = 0x9AA2D903;
        private const uint FileSignatureOld2 = 0xB54BFB65;


        public Task<IKdbTree> LoadAsync(StorageFile database, List<IUserKey> userKeys)
        {
            return LoadAsync(database, userKeys, new NullableProgress<double>());
        }

        public async Task<IKdbTree> LoadAsync(IStorageFile kdbDatabase, IList<IUserKey> userKeys, IProgress<double> percentComplete)
        {
            
            var kdbDataReader =  DataReader.FromBuffer(await FileIO.ReadBufferAsync(kdbDatabase));
            var versionInfo = ReadVersionInfo(kdbDataReader);
            IKdbReader reader = null;
            var compositeKey = new CompositeKey(userKeys, percentComplete);

            if (IsKdb4(versionInfo))
            {
                var pwDatabase = new PwDatabase(compositeKey);
                var kdb4File = new Kdb4File(pwDatabase);

                reader = new Kdb4Reader(kdb4File);      
            }
            else
            {
                throw new FormatException();
            }

            return await reader.Load(kdbDataReader);

        }

        private static bool IsKdb4(VersionInfo versionInfo)
        {
            return versionInfo.FileSignature1 == FileSignature1 && versionInfo.FileSignature2 == FileSignature2;
        }

        private VersionInfo ReadVersionInfo(IDataReader kdbReader)
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
