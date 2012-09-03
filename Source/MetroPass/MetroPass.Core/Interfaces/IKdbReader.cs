using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbReader
    {
        Task<IKdbTree> Load(IDataReader source);
    }
}
