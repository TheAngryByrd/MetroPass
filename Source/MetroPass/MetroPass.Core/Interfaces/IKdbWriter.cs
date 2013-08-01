using MetroPass.Core.Model;
using System.Threading.Tasks;
using Windows.Storage;

namespace MetroPass.Core.Interfaces
{
    public interface IKdbWriter
    {
        Task Write(PwDatabase tree, IStorageFile database);
    }
}
