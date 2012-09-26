using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;

namespace MetroPass.UI.ViewModels
{
    public class SearchResultsViewModel : BaseScreen
    {
        public SearchResultsViewModel(INavigationService navigationService) : base(navigationService)
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
                //SearchEntries();
            }
        }

        public ObservableCollection<PwEntry> Results { get; private set; }

        private void SearchEntries()
        {
            var root = PWDatabaseDataSource.Instance.PwDatabase.Tree.Group;

            var matchingEntries = root.AllEntries()
                .Where(e => e.Title.Contains(QueryText) || e.Notes.Contains(QueryText) || e.Username.Contains(QueryText));

            matchingEntries.ForEach(e => Results.Add(e));
        }
    }
}