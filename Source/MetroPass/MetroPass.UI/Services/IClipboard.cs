using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace MetroPass.UI.Services
{
    public interface IClipboard
    {
        Task CopyToClipboard(DataPackage data);
        Task ClearClipboard();
    }
}