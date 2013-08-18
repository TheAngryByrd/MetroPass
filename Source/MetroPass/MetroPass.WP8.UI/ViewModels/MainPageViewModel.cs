using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Caliburn.Micro;
using MetroPass.WP8.UI.Resources;

namespace MetroPass.WP8.UI.ViewModels
{
    public class MainPageViewModel : PropertyChangedBase
    {
        private readonly INavigationService _service;

        public MainPageViewModel(INavigationService service)
        {
            service.UriFor<DatabaseListViewModel>().Navigate();
        }

    }
}