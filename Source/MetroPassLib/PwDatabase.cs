using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroPassLib
{
    public class PwDatabase
    {
        public PwUuid DataCipherUuid { get; set; }

        public PwCompressionAlgorithm Compression { get; set; }

        public ulong KeyEncryptionRounds { get; set; }
    }
}
