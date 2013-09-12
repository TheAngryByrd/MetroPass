using System.Threading.Tasks;
using PCLStorage;

namespace Metropass.Core.PCL.Model.Kdb4.Writer
{
    public interface IKdbWriter
    {
        Task Write(PwDatabase tree, IFile database);
    }
}
