using Caliburn.Micro;
using MetroPass.UI.Services;

namespace MetroPass.UI.ViewModels
{
    public class StartPageViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;

        public StartPageViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPageServices pageServices)
            : base(navigationService, eventAggregator, pageServices)
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
