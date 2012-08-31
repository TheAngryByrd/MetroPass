using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace MetroPass.UI.Common
{
    public interface IDialogService
    {
        Task Show( string message);
        Task Show( string message,string title);
    }
    public class DialogService : IDialogService
    {
      

        public async Task Show(string title, string message)
        {
            var dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        public async Task Show(string message)
        {
            await Show(message, string.Empty);
        }
    }
}
