using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace MetroPass.UI.Services
{
    public class PageServices
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
