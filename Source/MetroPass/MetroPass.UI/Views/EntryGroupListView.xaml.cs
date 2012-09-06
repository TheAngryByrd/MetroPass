using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.Views
{
    public sealed partial class EntryGroupListView : Page
    {
        bool isNavigatedTo = false;
        public EntryGroupListView()
        {
            this.InitializeComponent();
        }

        //private void itemListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        //{
        //    if (!isNavigatedTo)
        //    {

        //        if (e.AddedItems.Count > 0)
        //        {
        //            var selectedItem = e.AddedItems.FirstOrDefault();
        //            if (selectedItem is PwEntry)
        //            {
        //                EntryAppBar.Visibility = Visibility.Visible;
        //                EntryAppBar.IsOpen = true;
        //            }
        //            else if (selectedItem is PwGroup)
        //            {
        //                Frame rootFrame = Window.Current.Content as Frame;
        //                rootFrame.Navigate(typeof(EntryGroupListPage), new EntryGroupListViewModel((PwGroup)selectedItem));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        this.itemListView.SelectedItem = null;
        //        isNavigatedTo = false;
        //    }

        //}


        //private void itemListView_ItemClick_1(object sender, ItemClickEventArgs e)
        //{
        //    this.itemListView.SelectedItem = e.ClickedItem;
        //}


        //private void EntryAppBar_Closed_1(object sender, object e)
        //{

        //    this.itemListView.SelectedItem = null;
        //}

        //private void EntryAppBar_Opened_1(object sender, object e)
        //{
        //    if (this.itemListView.SelectedItem == null)
        //    {
        //        EntryCommands.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        EntryCommands.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //    }
        //}


        //private async void CopyUsernameButton_Click_1(object sender, RoutedEventArgs e)
        //{
        //    var selectedItem = itemListView.SelectedItem;
        //    if (selectedItem != null)
        //    {
        //        var datapackage = new DataPackage();
        //        datapackage.SetText(((PwEntry)selectedItem).Username);
        //        Clipboard.SetContent(datapackage);
        //        await ClearClipboard();
        //    }

        //}

        //private async void CopyPasswordButton_Click_1(object sender, RoutedEventArgs e)
        //{
        //    var selectedItem = itemListView.SelectedItem;
        //    if (selectedItem != null)
        //    {
        //        var datapackage = new DataPackage();
        //        datapackage.SetText(((PwEntry)selectedItem).Password);
        //        Clipboard.SetContent(datapackage);
        //        await ClearClipboard();
        //    }
        //}

        //private Task ClearClipboard()
        //{
        //    var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        //    return Task.Factory.StartNew(async () =>
        //        {
        //            await Task.Delay(10000);
        //            Clipboard.Clear();
        //        }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);

           
        //}
    }
}
