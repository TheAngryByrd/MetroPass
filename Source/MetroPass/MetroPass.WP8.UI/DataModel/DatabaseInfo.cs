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
            _info = info;
        }

        private Info _info;

        public async Task Init()
        {
            

            await GetDatabase();
            await GetKeyfile();
        }

        private async Task GetKeyfile()
        {
            if (string.IsNullOrWhiteSpace(_info.KeyFilePath))
                return;

            KeyFile = await _folder.GetFileAsync(_info.KeyFilePath);
        }

        private async Task GetDatabase()
        {
            if (string.IsNullOrWhiteSpace(_info.DatabasePath))
                return;

            Database = await _folder.GetFileAsync(_info.DatabasePath);
        }

        public IStorageFile Database { get; set; }
        public IStorageFile KeyFile { get; set; }
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