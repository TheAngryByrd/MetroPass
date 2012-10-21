using System;

namespace MetroPass.UI.Views
{
    public interface IEntryEditView
    {
        void SetPasswordState(bool passwordsMatch);
    }
}