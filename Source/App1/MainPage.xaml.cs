using MetroPassLib;
using MetroPassLib.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public static StorageFile database;

        public MainPage()
        {
            Current = this;
            this.InitializeComponent();
            PickFilesButton.Click += new RoutedEventHandler(PickFilesButton_Click);
            OpenDatabase.Click += OpenDatabase_Click;
        }

        async void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            var bufferedData = await Windows.Storage.FileIO.ReadBufferAsync(database);
            PwDatabase pwDatabase = new PwDatabase();
              var composite = new CompositeKey();
            composite.UserKeys.Add(new KcpPassword(DatabasePassword.Password));
            pwDatabase.MasterKey = composite;
            Kdb4File kdb4 = new Kdb4File(pwDatabase);
            var tree = await kdb4.Load(DataReader.FromBuffer(bufferedData), Kdb4Format.Default);
        }

        private async void PickFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Current.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".kdbx");
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    // Application now has read/write access to the picked file
                    OutputTextBlock.Text = "Database: " + file.Name;
                    database = file;
                }
                else
                {
                    OutputTextBlock.Text = "Operation cancelled.";
                }
             

            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
