using MetroPass.Core.Model;
using MetroPass.Core.Model.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbWriter
    {
        Task Write(PwDatabase tree, IStorageFile database, CompositeKey compositeKey);
    }
}
