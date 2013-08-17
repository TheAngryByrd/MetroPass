using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.WhenNavigatedTo(OnNavigatedTo);
        }

        private IDisposable OnNavigatedTo()
        {
            return null;
        }

        public IScreen HostScreen
        {
            get; private set;
        }

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
