using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.WP8.UI.ViewModels
{
    public class OpenDatabaseViewModel
    {
        private readonly INavigationService _navigationService;

        public OpenDatabaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
