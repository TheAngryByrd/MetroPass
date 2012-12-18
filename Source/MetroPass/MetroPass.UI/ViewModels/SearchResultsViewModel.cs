using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Framework;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;

namespace MetroPass.UI.ViewModels
{
    public class SearchResultsViewModel : PasswordEntryScreen
    {
        public SearchResultsViewModel(INavigationService navigationService, IClipboard clipboard) : base(navigationService, clipboard)
        {
            Results = new ObservableCollection<PwEntry>();
        }

        public bool RedirectToLogin { get; set; }

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

        //If the query string comes in this way, it's because the search was activated without MetroPass already running
        //The user will need to login first before being able to perform their search
        public string Parameter
        {
            get { return _queryText; }
            set
            {
                QueryText = value;
                RedirectToLogin = true;
            }
        }

        public ObservableCollection<PwEntry> Results { get; private set; }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            SearchEntries();
        }

        private void SearchEntries()
        {
            if (RedirectToLogin)
            {
                SetState("NeedLogin");
                return;
            }

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