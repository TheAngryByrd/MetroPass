using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using MetroPass.WP8.UI.Services.Cloud;
using Windows.Storage;

namespace MetroPass.WP8.UI.DataModel
{
    public class DatabaseInfo
    {
        public readonly IStorageFolder Folder;

        public DatabaseInfo(IStorageFolder folder, Info info)
        {
            Folder = folder;
            Info = info;
        }

        public Info Info;

        public async Task<IStorageFile> GetKeyfile()
        {
            if (string.IsNullOrWhiteSpace(Info.KeyFilePath))
                return null;

            return await Folder.GetFileAsync(Info.KeyFilePath);
        }

        public async Task<IStorageFile> GetDatabase()
        {
            if (string.IsNullOrWhiteSpace(Info.DatabasePath))
                return null;

            return await Folder.GetFileAsync(Info.DatabasePath);
        }

    }

    public class Info
    {

        public readonly XDocument Document;

        private const string INFO = "Info";
        private const string DATABASE_PATH = "DatabasePath";
        private const string CLOUD_PROVIDER = "CloudProvider";
        private const string CLOUD_PATH = "CloudPath";
        private const string KEY_FILE_PATH = "KeyFilePath";


        public Info(XDocument document)
        {
            Document = document;
        }

        public static Info New()
        {
            var document = new XDocument();
            var info = new XElement(INFO);
            var database = new XElement(DATABASE_PATH);
            database.SetAttributeValue(CLOUD_PROVIDER, "");
            database.SetAttributeValue(CLOUD_PATH, "");
            info.Add(database);

            var keyfile = new XElement(KEY_FILE_PATH);
            info.Add(keyfile);

            document.Add(info);

            return new Info(document);
        }

        public XElement XInfo
        {
            get { return Document.Element(INFO); }
        }

        public string DatabasePath
        {
            get { return GetDatabaseElement().Value; }
            set { XInfo.SetElementValue(DATABASE_PATH, value); }
        }
  
        private XElement GetDatabaseElement()
        {
            return XInfo.Element(DATABASE_PATH);
        }

        public string DatabaseCloudProvider
        {
            get { return GetDatabaseElement().Attribute(CLOUD_PATH).Value; }
            set { GetDatabaseElement().SetAttributeValue(CLOUD_PATH, value); }
        }

        public string DatabaseCloudPath
        {

            get { return GetDatabaseElement().Attribute(CLOUD_PROVIDER).Value; }
            set { GetDatabaseElement().SetAttributeValue(CLOUD_PROVIDER, value); }
        }

        public string DatabaseUploadCloudPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DatabaseCloudProvider))
                    return string.Empty;
                string path = GetDatabaseElement().Attribute(CLOUD_PROVIDER).Value;
                if (DatabaseCloudProvider == CloudProvider.SkyDrive.ToString())
                {                    
                    return path;
                }
                else if (DatabaseCloudProvider == CloudProvider.Dropbox.ToString())
                {
                    return path.Remove(path.LastIndexOf('/')+1);
                }     
        
                return string.Empty;
            }

        }

        public string KeyFilePath
        {
            get { return XInfo.Element(KEY_FILE_PATH).Value; }
            set { XInfo.SetElementValue(KEY_FILE_PATH, value); }
        }

    }
}