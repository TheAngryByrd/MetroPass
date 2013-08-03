using Framework;
using MetroPass.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Windows.Storage;
using Metropass.Core.PCL;
using System.IO;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Metropass.Core.PCL.Model.Kdb4.Reader;
using MetroPass.WinRT.Infrastructure.Compression;

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
            var file = await FileIO.ReadBufferAsync(kdbDatabase);
            // var kdbDataReader =  DataReader.FromBuffer();
            MemoryStream kdbDataReader = new MemoryStream(file.AsBytes());
            var versionInfo = ReadVersionInfo(kdbDataReader);
            IKdbReader reader = null;
            var compositeKey = new CompositeKey(userKeys, percentComplete);
            var pwDatabase = new PwDatabase(compositeKey);

            if (IsKdb4(versionInfo))
            {
                var kdb4File = new Kdb4File(pwDatabase);

                reader = new Kdb4Reader(kdb4File,
                    new WinRTCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                    new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB),
                    new SHA256HasherRT(),
                    new GZipFactoryRT());

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
        public UInt32 FileSignature1 { get; set; }
        public UInt32 FileSignature2 { get; set; }
        public UInt32 Version { get; set; }

    }
}
