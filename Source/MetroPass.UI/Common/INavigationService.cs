using System;
namespace MetroPass.UI.Common
{
    public interface INavigationService
    {
        void GoBack();
        void GoForward();
        bool Navigate<T>(object parameter = null);
        bool Navigate(Type source, object parameter = null);
    }
}
