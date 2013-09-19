using System;
using Metropass.Core.PCL.Compression;
using Metropass.Core.PCL.Encryption;
using Metropass.Core.PCL.Hashing;

namespace Metropass.Core.PCL.Model.Kdb4.Writer
{
    public class KdbWriterFactory
    {
        private readonly IEncryptionEngine _databaseEncryptor;
        private readonly IKeyTransformer _keyTransformer;
        private readonly ICanSHA256Hash _hasher;
        private readonly IGZipStreamFactory _gzipFactory;
        public KdbWriterFactory(IEncryptionEngine databaseEncryptor, 
            IKeyTransformer keyTransformer, 
            ICanSHA256Hash hasher, 
            IGZipStreamFactory gzipFactory)
        {
            _gzipFactory = gzipFactory;
            _hasher = hasher;
            _keyTransformer = keyTransformer;
            _databaseEncryptor = databaseEncryptor;
        }

        public IKdbWriter CreateWriter(IKdbTree tree)
        {
            if (tree is Kdb4Tree)
            {

                return new Kdb4Writer(new Kdb4HeaderWriter(),
                    _databaseEncryptor,
                    _keyTransformer,
                    _hasher,
                    _gzipFactory);

            }
            else
            {
                throw new NotSupportedException();
            }

        }
    }
}


