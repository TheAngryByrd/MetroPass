using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.UI.Services;
using ReactiveUI;

namespace MetroPass.UI.ViewModels
{
    public class StartPageViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IDatabaseRepository _databaseRepository;
        private ReactiveList<KeepassFilePair> _keepassFilePairs = new ReactiveList<KeepassFilePair>();

        public StartPageViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPageServices pageServices,
            IDatabaseRepository databaseRepository)
            : base(navigationService, eventAggregator, pageServices)
        {
            _navigationService = navigationService;
            _databaseRepository = databaseRepository;
        }

        public void NewDatabase()
        {
            _navigationService.NavigateToViewModel(typeof(NewDatabaseViewModel));
        }

        public void OpenDatabase()
        {
            _navigationService.NavigateToViewModel(typeof(LoadKdbViewModel));
        }

        public ReactiveList<KeepassFilePair> KeepassFilePairs
        {
            get { return _keepassFilePairs; }
            set { _keepassFilePairs = value; }
        }

        protected async override Task OnActivate()
        {
            var recent = await _databaseRepository.GetRecentFiles();
            _keepassFilePairs.AddRange(recent);
        }


    }
}
