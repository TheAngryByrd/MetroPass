using MetroPassLib.Helpers;
using MetroPassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPassLib.Keys
{
    public class KcpPassword : IUserKey
    {
        private IBuffer keyData = null;
        public IBuffer KeyData
        {
            get { return keyData; }
        }

        public KcpPassword(string strPassword)
        {
            
            SetKey(UTF8Encoding.UTF8.GetBytes(strPassword).AsBuffer());
        }

        private async void SetKey(IBuffer passwordInUTF8)
        {
            var hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hashProvider.CreateHash();
            hash.Append(passwordInUTF8);
            var hashedBuffer = hash.GetValueAndReset();
            keyData = hashedBuffer;
        }
    }
}
