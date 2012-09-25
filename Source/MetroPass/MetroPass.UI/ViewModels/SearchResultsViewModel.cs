using System;
using System.Linq;
using Caliburn.Micro;

namespace MetroPass.UI.ViewModels
{
    public class SearchResultsViewModel : BaseScreen
    {
        public SearchResultsViewModel(INavigationService navigationService) : base(navigationService) { }

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
    }
}