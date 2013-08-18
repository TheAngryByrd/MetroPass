using System;
using System.Collections.ObjectModel;
using MetroPass.WP8.UI.Utils;
using MetroPass.WP8.UI.ViewModels.Interfaces;
using ReactiveUI;

namespace MetroPass.WP8.UI.ViewModels
{
    public class DatabaseListViewModel : ReactiveObject, IRoutableViewModel, IDatabaseListViewModel
    {
        public DatabaseListViewModel(IScreen screen)
        {
            HostScreen = screen;
            DatabaseNames = new ObservableCollection<string> { "Personal", "Work" };
         

            NavigateToLogin = new ReactiveCommand();
            NavigateToLogin.Subscribe(x =>
                {
                    screen.Router.Navigate.Navigate<SkydriveAccessViewModel>();
                });

        }

        public ReactiveCommand NavigateToLogin { 
            get; 
            private set; }

        public IScreen HostScreen
        {
            get; private set;
        }

        public Guid RandomGuid { get; set; }

        public ObservableCollection<string> DatabaseNames
        {
            get;
            set;
        }

        public string UrlPathSegment {
            get {
                return typeof(DatabaseListViewModel).Name;
            }
        }
    }
}
