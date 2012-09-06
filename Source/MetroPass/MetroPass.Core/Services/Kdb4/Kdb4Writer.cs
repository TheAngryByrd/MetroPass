using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Services.Kdb4
{
    public class Kdb4Writer :IKdbWriter
    {
        public void Write(PwDatabase tree, Windows.Storage.IStorageFile database, Model.Keys.CompositeKey compositeKey)
        {
            throw new NotImplementedException();
        }
    }
}
