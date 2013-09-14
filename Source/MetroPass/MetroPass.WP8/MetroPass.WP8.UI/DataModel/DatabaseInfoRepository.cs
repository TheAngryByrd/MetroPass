using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MetroPass.WP8.UI.DataModel
{
    public class DatabaseInfoRepository : IDatabaseInfoRepository
    {
        private StorageFolder _installedFolder;

        public DatabaseInfoRepository ()
	    {
            
            _installedFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
	    }

        public async Task DeleteKeyFile(DatabaseInfo databaseInfo)
        {         
            IStorageFile keyFile = await databaseInfo.GetKeyfile();
            await keyFile.DeleteAsync();

            databaseInfo.Info.KeyFilePath = "";
            await SaveInfo(databaseInfo.Folder, databaseInfo.Info);

        }

     

        public async Task<DatabaseInfo> GetDatabaseInfo(string databaseName)
        {
            var root = await GetDatabaseRoot();
            var folder = await root.GetFolderAsync(databaseName);

            return await CreateDatabaseInfo(folder);
        }

        public async Task SaveInfo(IStorageFolder folder, Info info)
        { 
            Stream stream = new MemoryStream();
            info.Document.Save(stream);

            stream.Position = 0;

            await WriteFile("Info.info", folder, stream);
        }

        public async Task<Info> GetInfo(IStorageFolder folder)
        {

            Info retval = null;
            bool found = true;
            StorageFile info = null;
            try
            {               
                info = await folder.GetFileAsync("Info.info");
            }
            catch(FileNotFoundException fnf)
            {
                found = false;
            }

            if (!found)
            {
                var doc = new XDocument();                
                retval = new Info(doc);     
                await SaveInfo(folder, retval);
            }
            else
            {
                using(var infoStream = await info.OpenReadAsync())
                using(Stream stream = infoStream.AsStream())
                {
                    XDocument document = XDocument.Load(stream);
                    retval = new Info(document);
                }       
            }

            return retval;
           
        }

        public async Task<IEnumerable<DatabaseInfo>> GetDatabaseInfo()
        { 
            var root = await GetDatabaseRoot();
            var databsesFolders = await root.GetFoldersAsync();
            var DatabaseItems = new List<DatabaseInfo>();
            foreach (var folder in databsesFolders)
            {
                var databaseInfo = await CreateDatabaseInfo(folder);
                DatabaseItems.Add(databaseInfo);
            }

            return DatabaseItems;
        }
  
        private async Task<DatabaseInfo> CreateDatabaseInfo(StorageFolder folder)
        {
            var databaseInfo = new DatabaseInfo(folder, await GetInfo(folder));
            return databaseInfo;
        }
  
        private async Task<StorageFolder> GetDatabaseRoot()
        {
            var root = await _installedFolder.CreateFolderAsync("Databases", CreationCollisionOption.OpenIfExists);
            return root;
        }

        public async Task SaveDatabaseFromDatasouce(
            string databaseName, 
            string cloudprovider, 
            string cloudPath, 
            string cloudUploadPath,
            Stream database)
        {
            var root = await _installedFolder.CreateFolderAsync("Databases", CreationCollisionOption.OpenIfExists);
            var folder = await root.CreateFolderAsync(databaseName, CreationCollisionOption.OpenIfExists);
            var info = await GetInfo(folder);

            info.DatabasePath = databaseName;
            info.DatabaseCloudProvider = cloudprovider;
            info.DatabaseCloudPath = cloudPath;
            info.DatabaseCloudPath = cloudUploadPath;
            await SaveInfo(folder, info);

            await WriteFile(databaseName, folder, database);
        }
 
        public async Task SaveKeyFileFromDatasouce(string databaseName, string keyFileName, Stream keyFile)
        {
            var root = await _installedFolder.CreateFolderAsync("Databases", CreationCollisionOption.OpenIfExists);
            var folder = await root.CreateFolderAsync(databaseName, CreationCollisionOption.OpenIfExists);
            var info = await GetInfo(folder);

            info.KeyFilePath = keyFileName;
            await SaveInfo(folder, info);

            await WriteFile(keyFileName, folder, keyFile);
        }


        private async Task WriteFile(string fileName, IStorageFolder folder, Stream data)
        {

            try
            {
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using (data)
                using (var writeStream = await file.OpenStreamForWriteAsync())
                {
                    await writeStream.WriteAsync(data.ToArray(), 0, (int)data.Length);
                    await writeStream.FlushAsync();
                }
                
            }
            catch(Exception e)
            {
                
            }

        }
    }
}
