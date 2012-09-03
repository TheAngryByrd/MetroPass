using Framework;
using MetroPass.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MetroPass.Core.Model.Keys
{
    public class KcpKeyFile : IUserKey
    {
        private IBuffer _keyData;
        public IBuffer KeyData
        {
            get { return _keyData; }
        }

        private KcpKeyFile()
        {
        }

        public static async Task<KcpKeyFile> Create(IStorageFile storageFile)
        {
            var kcpKeyFile = new KcpKeyFile();
            await kcpKeyFile.Init(storageFile);
            return kcpKeyFile;
        }

        private async Task Init(IStorageFile storageFile)
        {
            IBuffer pbKey = await LoadXmlKeyFile(storageFile);
            if (pbKey == null) pbKey = LoadKeyFile(await FileIO.ReadBufferAsync(storageFile));

            if (pbKey == null) throw new InvalidOperationException();

            _keyData = pbKey;
        }

        private static IBuffer LoadKeyFile(IBuffer pbFileData)
        {


            uint iLength = pbFileData.Length;

            IBuffer pbKey = null;

            if (iLength == 32) pbKey = LoadBinaryKey32(pbFileData);
            else if (iLength == 64) pbKey = LoadHexKey32(pbFileData);

            if (pbKey == null)
            {
                pbKey = SHA256Hasher.Hash(pbFileData);

            }

            return pbKey;
        }
        private static IBuffer LoadBinaryKey32(IBuffer pbFileData)
        {
            if (pbFileData == null) { return null; }
            if (pbFileData.Length != 32) { return null; }

            return pbFileData;
        }

        private static IBuffer LoadHexKey32(IBuffer pbFileData)
        {
            if (pbFileData == null) { return null; }
            if (pbFileData.Length != 64) { return null; }

            try
            {

                string strHex = Encoding.Unicode.GetString(pbFileData.AsBytes(), 0, 64);
                if (!StrUtil.IsHexString(strHex, true)) return null;

                IBuffer pbKey = MemUtil.HexStringToByteArray(strHex);
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

        private async static Task<IBuffer> LoadXmlKeyFile(IStorageFile file)
        {
            byte[] pbKeyData = null;

            try
            {
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);


                XmlElement el = doc.DocumentElement;

                if ((el == null) || !el.NodeName.Equals(RootElementName)) return null;
                if (el.ChildNodes.Count < 2) return null;

                foreach (var xmlChild in el.ChildNodes)
                {
                    if (xmlChild.NodeName.Equals(MetaElementName)) { } // Ignore Meta
                    else if (xmlChild.NodeName == KeyElementName)
                    {
                        foreach (IXmlNode xmlKeyChild in xmlChild.ChildNodes)
                        {
                            if (xmlKeyChild.NodeName == KeyDataElementName)
                            {
                                if (pbKeyData == null)
                                    pbKeyData = Convert.FromBase64String(xmlKeyChild.InnerText);
                            }
                        }
                    }
                }
            }
            catch (Exception) { pbKeyData = null; }


            return pbKeyData.AsBuffer();
        }
    }
}
