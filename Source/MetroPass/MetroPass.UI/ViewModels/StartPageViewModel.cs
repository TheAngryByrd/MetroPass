using Caliburn.Micro;
using MetroPass.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.UI.ViewModels
{
    public class StartPageViewModel : BaseScreen
    {
        private readonly IPageServices _pageServices;
        private readonly INavigationService _navigationService;

        public StartPageViewModel(IPageServices pageServices, INavigationService navigationService)
            : base(navigationService)
        {
            _pageServices = pageServices;
            _navigationService = navigationService;
        }

        public void OpenDatabase()
        {
            _navigationService.NavigateToViewModel(typeof(LoadKdbViewModel));
        }
    }
}
