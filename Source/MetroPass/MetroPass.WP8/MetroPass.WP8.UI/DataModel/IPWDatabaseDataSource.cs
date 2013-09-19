using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4.Keys;
using Windows.Storage;

namespace MetroPass.UI.DataModel
{
    public interface IPWDatabaseDataSource
    {
        IStorageFile StorageFile { get; set; }

        PwDatabase PwDatabase { get; set; }

        Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys);

        Task LoadPwDatabase(IStorageFile pwDatabaseFile, IList<IUserKey> userKeys, IProgress<double> percentComplete);

        Task SavePwDatabase();
    }
}