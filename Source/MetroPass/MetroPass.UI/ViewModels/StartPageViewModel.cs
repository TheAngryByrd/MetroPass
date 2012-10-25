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

        private readonly INavigationService _navigationService;

        public StartPageViewModel( INavigationService navigationService)
            : base(navigationService)
        {

            _navigationService = navigationService;
        }

        public void NewDatabase()
        {
            _navigationService.NavigateToViewModel(typeof(NewDatabaseViewModel));
        }

        public void OpenDatabase()
        {
            _navigationService.NavigateToViewModel(typeof(LoadKdbViewModel));
        }
    }
}
