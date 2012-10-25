using System;

namespace MetroPass.UI.Views
{
    public interface IPasswordErrorStateView
    {
        void SetPasswordState(bool passwordsMatch);
    }
}