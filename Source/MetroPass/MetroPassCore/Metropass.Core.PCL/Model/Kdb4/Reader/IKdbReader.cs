using System.IO;
using System.Threading.Tasks;

namespace Metropass.Core.PCL.Model.Kdb4.Reader
{
    public interface IKdbReader
    {
        Task<IKdbTree> Load(Stream source);
    }
}
