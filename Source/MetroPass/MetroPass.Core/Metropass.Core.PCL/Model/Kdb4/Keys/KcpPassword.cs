using System.Text;
using System.Threading.Tasks;
using Metropass.Core.PCL.Hashing;

namespace Metropass.Core.PCL.Model.Kdb4.Keys
{
    public class KcpPassword : IUserKey
    {
        private byte[] keyData = null;
        public byte[] KeyData
        {
            get
            {
                return keyData;
            }
        }
        private KcpPassword(){ }

        public async static Task<KcpPassword> Create(string password, ICanSHA256Hash hasher)
        {
            var kcpPassword = new KcpPassword();

            await kcpPassword.Init(UTF8Encoding.UTF8.GetBytes(password), hasher);

            return kcpPassword;
        }

        private async Task Init(byte[] passwordInUTF8,ICanSHA256Hash hasher)
        {
            var hashedBuffer = hasher.Hash(passwordInUTF8);
            keyData = hashedBuffer;
        }

 
    }
}
