using System;
using System.Linq;
using Caliburn.Micro;

namespace MetroPass.UI.ViewModels
{
    public class BaseScreen : Screen
    {
        private readonly INavigationService _navigationService;

        public BaseScreen(INavigationService navigationService)
        {
            this._navigationService=navigationService;
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public bool CanGoBack
        {
            get { return _navigationService.CanGoBack; }
        }
    }
}