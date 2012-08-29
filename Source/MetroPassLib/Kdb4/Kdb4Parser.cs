using MetroPassLib.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MetroPassLib.Kdb4
{
    public class Kdb4Parser
    {
        public CryptoRandomStream _cryptoStream { get; set; }
        public Kdb4Parser(CryptoRandomStream crypto)
        {
            _cryptoStream = crypto;
        }

        public Kdb4Tree Parse(IDataReader decrypredDatabase)
        {

            throw new NotImplementedException();
        }
    }
}
