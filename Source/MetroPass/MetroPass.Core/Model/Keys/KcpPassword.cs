using MetroPass.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MetroPass.Core.Model.Keys
{
    public class KcpPassword : IUserKey
    {
        private IBuffer keyData = null;
        public IBuffer KeyData
        {
            get { return keyData; }
        }

        private KcpPassword()
        {


        }

        public async static Task<KcpPassword> Create(string password)
        {
            var kcpPassword = new KcpPassword();

            await kcpPassword.Init(UTF8Encoding.UTF8.GetBytes(password).AsBuffer());

            return kcpPassword;
        }

        private async Task Init(IBuffer passwordInUTF8)
        {


            var hashedBuffer = SHA256Hasher.Hash(passwordInUTF8);
            keyData = hashedBuffer;
        }

 
    }
}
