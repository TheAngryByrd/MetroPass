using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Caliburn.Micro;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels.DesignTime;
using MetroPass.WP8.UI.ViewModels.Interfaces;
using ReactiveUI;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DatabaseListViewModel : IDatabaseListViewModel
    {
        private readonly INavigationService _navService;

        public DatabaseListViewModel(INavigationService navService)
        { 
            _navService = navService;
            DatabaseNames = new ObservableCollection<string> { "Personal", "Work" };
            NavigateToLoginCommand = new ReactiveCommand();
            NavigateToLoginCommand.Subscribe(NavigateToLogin);
        }
     

        public ObservableCollection<string> DatabaseNames
        {
            get;
            set;
        }

        public IReactiveCommand NavigateToLoginCommand { get; private set; }

        public void NavigateToLogin(object obj)
        {
            _navService.UriFor<EntriesListViewModel>().Navigate();
        }
    }
}
