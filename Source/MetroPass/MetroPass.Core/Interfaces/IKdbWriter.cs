using MetroPass.Core.Model;
using MetroPass.Core.Model.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbWriter
    {
        void Write(PwDatabase tree, IStorageFile database, CompositeKey compositeKey);
    }
}
