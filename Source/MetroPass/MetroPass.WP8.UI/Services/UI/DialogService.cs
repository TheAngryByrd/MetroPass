using System;
using Microsoft.Phone.Controls;

namespace MetroPass.WP8.UI.Services.UI
{
    public class DialogService : IDialogService
    {
        public void ShowDialogBox(string caption, string message, string leftbuttonContent, string rightButtonContent, Action leftButtonAction, Action rightButtonAction)
        {
            var messagebox = new CustomMessageBox()
            {
                Caption = caption,
                Message = message,
                LeftButtonContent = leftbuttonContent,
                RightButtonContent = rightButtonContent
            };

            messagebox.Dismissed += (s,e) =>
            {
                switch (e.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        leftButtonAction();
                        break;
                    case CustomMessageBoxResult.RightButton:
                        rightButtonAction();
                        break;
                    case CustomMessageBoxResult.None:
                        break;
                };
            };

            messagebox.Show();
        }
  
    }
}