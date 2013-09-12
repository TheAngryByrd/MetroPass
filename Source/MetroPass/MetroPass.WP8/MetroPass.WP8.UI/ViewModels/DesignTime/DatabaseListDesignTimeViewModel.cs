using System;
using System.Collections.ObjectModel;
using MetroPass.WP8.UI.ViewModels.Interfaces;

namespace MetroPass.WP8.UI.ViewModels.DesignTime
{
    public class DatabaseListDesignTimeViewModel : IDatabaseListViewModel
    {
        public ObservableCollection<string> DatabaseItems { get; set; }

        public DatabaseListDesignTimeViewModel()
        {
            DatabaseItems = new ObservableCollection<string> { "Personal", "Work" };
        }
        ObservableCollection<DatabaseItemViewModel> IDatabaseListViewModel.DatabaseItems { get; set; }
    }
}
