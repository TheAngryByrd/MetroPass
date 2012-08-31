
using MetroPass.UI.Common;
using MetroPass.UI.ViewModels;
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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MetroPass.UI
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DatabaseLoad : MetroPass.UI.Common.LayoutAwarePage
    {
        public static DatabaseLoad Current;

        public static StorageFile database;
        public static StorageFile keyFile;
        public DatabaseLoad()
      
        {
           
            Current = this;
            this.InitializeComponent();
            this.DataContext = new DatabaseLoadViewModel(new DialogService());
            //FilePickerDatabaseButton.Click += new RoutedEventHandler(PickFileDatabaseButton_Click);
            //FilePickerKeyFileButton.Click += FilePickerKeyFileButton_Click;
            //OpenDatabase.Click += OpenDatabase_Click;
        }


        private async void FilePickerKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (Current.EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".key");
                var file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    // Application now has read/write access to the picked file
                    OutputTextBlockKeyFile.Text = "Keyfile: " + file.Name;
                    keyFile = file;
                }
                else
                {
                    OutputTextBlockKeyFile.Text = "Operation cancelled.";
                }


            }
        }

        async void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            var bufferedData = await Windows.Storage.FileIO.ReadBufferAsync(database);
            PwDatabase pwDatabase = new PwDatabase();
            var composite = new CompositeKey();
            if(!string.IsNullOrEmpty(DatabasePassword.Password))
            {
                composite.UserKeys.Add(await KcpPassword.Create(DatabasePassword.Password));
            }
            if(keyFile != null)
            {
                composite.UserKeys.Add(await KcpKeyFile.Create(keyFile));
            }
      
            pwDatabase.MasterKey = composite;
            Kdb4File kdb4 = new Kdb4File(pwDatabase);
            var tree = await kdb4.Load(DataReader.FromBuffer(bufferedData), Kdb4Format.Default);

                        
        }

        private async void PickFileDatabaseButton_Click(object sender, RoutedEventArgs e)
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
                    OutputTextBlockDatabaseFile.Text = "Database: " + file.Name;
                    database = file;
                }
                else
                {
                    OutputTextBlockDatabaseFile.Text = "Operation cancelled.";
                }


            }
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
