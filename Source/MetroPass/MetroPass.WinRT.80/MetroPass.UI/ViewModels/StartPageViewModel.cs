using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.UI.Common;
using MetroPass.UI.Services;
using ReactiveUI;
using System;

namespace MetroPass.UI.ViewModels
{
    public class StartPageViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IDatabaseRepository _databaseRepository;
        private ObservableCollection<KeepassFilePair> _keepassFilePairs = new ObservableCollection<KeepassFilePair>();
        private KeepassFilePair _selectedKeepassFilePair;

        public StartPageViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPageServices pageServices,
            IDatabaseRepository databaseRepository)
            : base(navigationService, eventAggregator, pageServices)
        {
            _navigationService = navigationService;
            _databaseRepository = databaseRepository;
            NewDatabaseCommand = new ReactiveCommand();
      
            NewDatabaseCommand.Subscribe(_ => NewDatabase());
            OpenDatabaseCommand = new ReactiveCommand();
            OpenDatabaseCommand.Subscribe(_ => OpenDatabase());

            RemoveDatabaseCommand = new ReactiveCommand();
            RemoveDatabaseCommand.Subscribe(_ => RemoveDatabase());

            DeleteDatabaseCommand = new ReactiveCommand();
        }

        private async void RemoveDatabase()
        {
            await _databaseRepository.Delete(SelectedKeepassFilePair);

            _keepassFilePairs.Remove(SelectedKeepassFilePair);
            SelectedKeepassFilePair = new KeepassFilePair();
        }

        public ReactiveCommand OpenDatabaseCommand { get; private set; }
        public ReactiveCommand NewDatabaseCommand { get; private set; }
        public ReactiveCommand RemoveDatabaseCommand { get; private set; }
        public ReactiveCommand DeleteDatabaseCommand { get; private set; }

        public void NewDatabase()
        {
            _navigationService.UriFor<NewDatabaseViewModel>().Navigate();
        }

        public void OpenDatabase()
        {
            _navigationService
                .UriFor<LoadKdbViewModel>()
                .WithParam(vm => vm.KeepassFileTokenPairDatabase, SelectedKeepassFilePair.TokenPair.DatabaseFileToken ?? Guid.NewGuid().ToString())
                .WithParam(vm => vm.KeepassFileTokenPairKeeFile, SelectedKeepassFilePair.TokenPair.KeeFileToken ?? Guid.NewGuid().ToString())
                .Navigate();
        }

        public ObservableCollection<KeepassFilePair> KeepassFilePairs
        {
            get { return _keepassFilePairs; }
            set { _keepassFilePairs = value; }
        }

        protected async override Task OnActivate()
        {
            await PopulateRecentFiles();

            this.WhenAnyValue(x => x.SelectedKeepassFilePair)
                .Subscribe(_ => ItemSelected = SelectedKeepassFilePair.Database != null);
        }

        private async Task PopulateRecentFiles()
        {
            var recent = await _databaseRepository.GetRecentFiles();
            _keepassFilePairs.AddRange(recent);
        }

        private bool _itemSelected;
        public bool ItemSelected
        {
            get { return _itemSelected; }
            set { this.RaiseAndSetIfChanged(ref _itemSelected, value); }
        }

        public KeepassFilePair SelectedKeepassFilePair
        {
            get { return _selectedKeepassFilePair; }
            set { this.RaiseAndSetIfChanged(ref _selectedKeepassFilePair, value); }
        }
    }
}
