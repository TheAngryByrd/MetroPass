using System.Collections.ObjectModel;
using MetroPass.WP8.UI.ViewModels.Interfaces;

namespace MetroPass.WP8.UI.ViewModels.DesignTime
{
    public class DatabaseListDesignTimeViewModel : IDatabaseListViewModel
    {
        public ObservableCollection<string> DatabaseNames { get; set; }

        public DatabaseListDesignTimeViewModel()
        {
            DatabaseNames = new ObservableCollection<string> { "Personal", "Work" };
        }
    }
}
