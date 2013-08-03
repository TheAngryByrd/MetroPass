using System.Threading.Tasks;
using Metropass.Core.PCL.Model;
using PCLStorage;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbWriter
    {
        Task Write(PwDatabase tree, IFile database);
    }
}
