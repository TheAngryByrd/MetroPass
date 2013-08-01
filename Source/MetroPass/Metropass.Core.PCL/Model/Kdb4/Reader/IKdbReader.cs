using System.IO;
using System.Threading.Tasks;
using Metropass.Core.PCL.Model.Kdb4;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbReader
    {
        Task<IKdbTree> Load(Stream source);
    }
}
