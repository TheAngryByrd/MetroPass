using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using Metropass.Core.PCL.Model;

namespace MetroPass.UI.ViewModels
{
    public class SearchResultsViewModel : PasswordEntryScreen
    {
        public SearchResultsViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IClipboard clipboard,
            IPageServices pageServices)
            : base(navigationService, eventAggregator, clipboard, pageServices)
        {
            Results = new ObservableCollection<PwEntry>();
        }

        private string _queryText;
        public string QueryText
        {
            get { return this._queryText; }
            set
            {
                _queryText = value;
                NotifyOfPropertyChange(() => QueryText);
            }
        }

        public ObservableCollection<PwEntry> Results { get; private set; }

        protected internal override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            SearchEntries();
        }

        private void SearchEntries()
        {
            var root = PWDatabaseDataSource.Instance.PwDatabase.Tree.Group;

            var matchingEntries = root.AllEntries()
                .Where(e => e.Title.ContainsInsensitive(QueryText) || e.Notes.ContainsInsensitive(QueryText) || e.Username.ContainsInsensitive(QueryText));

            matchingEntries.OrderBy(e => e.Title).ForEach(e => Results.Add(e));

            if (Results.Count == 0)
            {
                SetState("NoResultsFound");
            }
        }
    }
}