using System.Collections.Generic;
using System.Windows;
using MetroPass.WP8.UI.ViewModels;
using Microsoft.Phone.Controls;
using ReactiveUI;

namespace MetroPass.WP8.UI.Views
{
    public partial class DatabaseListView : PhoneApplicationPage, IViewFor<DatabaseListViewModel>  
    {
        public DatabaseListView()
        {
            InitializeComponent();
            this.OneWayBind(ViewModel, vm => vm.DatabaseNames, v => v.DatabaseList.ItemsSource);
        }   


        public DatabaseListViewModel ViewModel
        {
            get { return (DatabaseListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(DatabaseListViewModel), typeof(DatabaseListView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DatabaseListViewModel)value; }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
    }
}