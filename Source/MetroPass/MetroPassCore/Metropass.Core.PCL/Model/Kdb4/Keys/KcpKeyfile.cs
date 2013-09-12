using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metropass.Core.PCL.Helpers;
using System.Xml.Linq;
using PCLStorage;
using System.IO;
using Metropass.Core.PCL.Hashing;

namespace Metropass.Core.PCL.Model.Kdb4.Keys
{
    public class KcpKeyFile : IUserKey
    {
        private byte[] _keyData; 

        public byte[] KeyData
        {
            get { return _keyData; }
        }

        private KcpKeyFile()
        {
        }

        public static ICanSHA256Hash _hasher;
        public static async Task<KcpKeyFile> Create(IFile storageFile, ICanSHA256Hash hasher)
        {
            var kcpKeyFile = new KcpKeyFile();
            _hasher = hasher;
            await kcpKeyFile.Init(storageFile);
            return kcpKeyFile;
        }

        private async Task Init(IFile storageFile)
        {
            var fileAsStream = await storageFile.OpenAsync(FileAccess.Read);
            byte[] pbKey = await LoadXmlKeyFile(fileAsStream);
            if (pbKey == null) pbKey = LoadKeyFile(fileAsStream.ToArray());

            if (pbKey == null) throw new InvalidOperationException();

            _keyData = pbKey;
        }

        private static byte[] LoadKeyFile(byte[] pbFileData)
        {


            int iLength = pbFileData.Length;

            byte[] pbKey = null;

            if (iLength == 32) pbKey = LoadBinaryKey32(pbFileData);
            else if (iLength == 64) pbKey = LoadHexKey32(pbFileData);

            if (pbKey == null)
            {
                pbKey = _hasher.Hash(pbFileData);

            }

            return pbKey;
        }
        private static byte[] LoadBinaryKey32(byte[] pbFileData)
        {
            if (pbFileData == null) { return null; }
            if (pbFileData.Length != 32) { return null; }

            return pbFileData;
        }

        private static byte[] LoadHexKey32(byte[] pbFileData)
        {
            if (pbFileData == null) { return null; }
            if (pbFileData.Length != 64) { return null; }

            try
            {

                string strHex = Encoding.Unicode.GetString(pbFileData, 0, 64);
                if (!StrUtil.IsHexString(strHex, true)) return null;

                byte[] pbKey = MemUtil.HexStringToByteArray(strHex);
                if ((pbKey == null) || (pbKey.Length != 32))
                    return null;

                return pbKey;
            }
            catch (Exception) { }

            return null;
        }


        private const string RootElementName = "KeyFile";
        private const string MetaElementName = "Meta";
        private const string VersionElementName = "Version";
        private const string KeyElementName = "Key";
        private const string KeyDataElementName = "Data";

        private async static Task<byte[]> LoadXmlKeyFile(Stream file)
        {
            byte[] pbKeyData = null;

            try
            {
                var doc = XDocument.Load(file).Document;

                var keyFileNode = doc.Document.Element(RootElementName);
                
                if (keyFileNode == null) return null;
                if (keyFileNode.Elements().Count() < 2) return null;
                var dataElement = keyFileNode.Descendants(KeyDataElementName).FirstOrDefault();
                if (dataElement != null && pbKeyData == null)
                {
                    pbKeyData = Convert.FromBase64String(dataElement.Value);
                }
            }
            catch (Exception e) { pbKeyData = null; }


            return pbKeyData;
        }
    }
}
