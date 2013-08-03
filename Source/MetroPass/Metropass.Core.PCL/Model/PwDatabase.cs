using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.Model.Kdb4.Keys;

namespace Metropass.Core.PCL.Model
{
    public class PwDatabase
    {
        public PwDatabase(CompositeKey masterKey)
        {
            MasterKey = masterKey;
        }
        public PwUuid DataCipherUuid { get; set; }

        public PwCompressionAlgorithm Compression { get; set; }

        public ulong KeyEncryptionRounds { get; set; }

        public CompositeKey MasterKey { get; set; }

        public IKdbTree Tree { get; set; }
        
    }
}
