using MetroPass.Core.Interfaces;
using MetroPass.Core.Model.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Model
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
