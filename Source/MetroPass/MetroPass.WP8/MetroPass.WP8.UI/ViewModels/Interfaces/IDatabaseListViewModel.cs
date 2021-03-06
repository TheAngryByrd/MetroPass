using System.Collections.ObjectModel;

namespace MetroPass.WP8.UI.ViewModels.Interfaces
{
    public interface IDatabaseListViewModel
    {
        ObservableCollection<DatabaseItemViewModel> DatabaseItems {
        get;
        set;
        }
    }
}