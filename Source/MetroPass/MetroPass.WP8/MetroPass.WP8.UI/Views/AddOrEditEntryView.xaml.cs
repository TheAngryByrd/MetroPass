using Microsoft.Phone.Controls;

namespace MetroPass.WP8.UI.Views
{
    public partial class AddOrEditEntryView : PhoneApplicationPage
    {
        public AddOrEditEntryView()
        {
            InitializeComponent();
            Loaded += AddOrEditEntryView_Loaded;
        }

        void AddOrEditEntryView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ItemView1_AppBar1.IsVisible = true;
            ItemView1_AppBar2.IsVisible = false;
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ItemView1_AppBar1.IsVisible = false;
            ItemView1_AppBar2.IsVisible = false;

            var item = e.AddedItems[0] as PivotItem;
            if(item.Header.Equals("entry"))
            {
                ItemView1_AppBar1.IsVisible = true;
            }
            else if (item.Header.Equals("custom fields"))
            {
                ItemView1_AppBar2.IsVisible = true;
            }
        }
  
    }
}