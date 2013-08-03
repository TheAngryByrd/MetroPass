using MetroPass.Core.Interfaces;
using MetroPass.Core.Services.Kdb4.Writer;
using System;
using MetroPass.WinRT.Infrastructure.Compression;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;

namespace MetroPass.Core.Services
{
    public class KdbWriterFactory
    {
        public IKdbWriter CreateWriter(IKdbTree tree)
        {
            if (tree is Kdb4Tree)
            {
                return new Kdb4Writer(new Kdb4HeaderWriter(),
                      new WinRTCrypto(CryptoAlgoritmType.AES_CBC_PKCS7),
                      new MultiThreadedBouncyCastleCrypto(CryptoAlgoritmType.AES_ECB),
                      new SHA256HasherRT(),
                      new GZipFactoryRT());
            }
            else
            {
                throw new NotSupportedException();
            }

        }
    }
}
