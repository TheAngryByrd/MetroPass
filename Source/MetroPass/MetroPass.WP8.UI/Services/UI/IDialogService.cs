using System;

namespace MetroPass.WP8.UI.Services.UI
{
    public interface IDialogService
    {
        void ShowDialogBox(string caption, string message, string leftbutton, string rightButton, Action ok, Action cancel);
    }
}
