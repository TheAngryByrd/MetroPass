using MetroPassLib.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroPassLib
{
    public class PwDatabase
    {
        public PwDatabase()
        {
            MasterKey = new CompositeKey();
        }

        public PwUuid DataCipherUuid { get; set; }

        public PwCompressionAlgorithm Compression { get; set; }

        public ulong KeyEncryptionRounds { get; set; }

        public CompositeKey MasterKey { get; set; }
    }
}
