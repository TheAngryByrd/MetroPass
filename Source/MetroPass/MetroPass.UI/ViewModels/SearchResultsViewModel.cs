using System;
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

        private string _queryText;
        public string QueryText
        {
            get { return this._queryText; }
            set
            {
                this._queryText = value;
                NotifyOfPropertyChange(() => QueryText);
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