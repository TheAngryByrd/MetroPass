using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI.Services
{
    public interface IDialogService
    {
        void ShowDialogBox(string caption, string message, string leftbutton, string rightButton, Action ok, Action cancel);
    }
}
