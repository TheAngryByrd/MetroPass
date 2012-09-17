using System;
using System.Linq;
using System.Threading.Tasks;

namespace MetroPass.UI.Services
{
    public interface IClipboard
    {
        Task CopyToClipboard(string textToCopy);
        Task ClearClipboard();
    }
}