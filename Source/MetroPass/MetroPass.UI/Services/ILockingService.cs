using System;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.ViewModels;

namespace MetroPass.UI.Services
{
    public interface ILockingService
    {
        void Lock();

        void OnResumeLock(DateTime suspendedDate);
    }
}