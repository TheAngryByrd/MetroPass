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
             _installedFolder = Package.Current.InstalledLocation;
	    }
        

        private async Task SaveInfo(IStorageFolder folder, Info info)
        { 
            Stream stream = new MemoryStream();
            info.Document.Save(stream);

            stream.Position = 0;

            await WriteFile("Info.info", folder, stream);
        }

        private async Task<Info> GetInfo(IStorageFolder folder)
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
            var root = await _installedFolder.CreateFolderAsync("Databases", CreationCollisionOption.OpenIfExists);
            var databsesFolders = await root.GetFoldersAsync();
            var DatabaseItems = new List<DatabaseInfo>();
            foreach (var folder in databsesFolders)
            {
                var databaseInfo = new DatabaseInfo(folder, await GetInfo(folder));
                DatabaseItems.Add(databaseInfo);
                await databaseInfo.Init();
            }

            return DatabaseItems;
        }

        public async Task SaveDatabaseFromDatasouce(string databaseName, Stream database)
        {
            var root = await _installedFolder.CreateFolderAsync("Databases", CreationCollisionOption.OpenIfExists);
            var folder = await root.CreateFolderAsync(databaseName, CreationCollisionOption.OpenIfExists);
            var info = await GetInfo(folder);

            info.DatabasePath = databaseName;
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
