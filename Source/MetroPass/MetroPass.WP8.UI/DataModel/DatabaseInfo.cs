using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace MetroPass.WP8.UI.DataModel
{
    public class DatabaseInfo
    {
        private readonly IStorageFolder _folder;

        public DatabaseInfo(IStorageFolder folder, Info info)
        {
            _folder = folder;
            Info = info;
        }

        public Info Info;

        public async Task<IStorageFile> GetKeyfile()
        {
            if (string.IsNullOrWhiteSpace(Info.KeyFilePath))
                return null;

            return await _folder.GetFileAsync(Info.KeyFilePath);
        }

        public async Task<IStorageFile> GetDatabase()
        {
            if (string.IsNullOrWhiteSpace(Info.DatabasePath))
                return null;

            return await _folder.GetFileAsync(Info.DatabasePath);
        }

    }

    public class Info
    {

        public readonly XDocument Document;

        public Info(XDocument document)
        {
            Document = document;

            XElement element = document.Element("Info");
            if(element == null)
            {
                var info = new XElement("Info");
                info.Add(new XElement("DatabasePath"));
                info.Add(new XElement("KeyFilePath"));

                document.Add(info);
            }
        }

        public XElement XInfo
        {
            get { return Document.Element("Info"); }
        }

        public string DatabasePath
        {
            get { return XInfo.Element("DatabasePath").Value; }
            set { XInfo.SetElementValue("DatabasePath", value); }
        }

        public string KeyFilePath
        {
            get { return XInfo.Element("KeyFilePath").Value; }
            set { XInfo.SetElementValue("KeyFilePath", value); }
        }

    }
}