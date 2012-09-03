﻿using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using MetroPass.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class LoadKdbPage : MetroPass.UI.Common.LayoutAwarePage
    {
       public static LoadKdbPage Current;


        public LoadKdbPage()
      
        {
           
            Current = this;
            this.InitializeComponent();
            this.DataContext = new LoadKdbPageViewModel(new PageServices(), App.NavigationService);
            //FilePickerDatabaseButton.Click += new RoutedEventHandler(PickFileDatabaseButton_Click);
            //FilePickerKeyFileButton.Click += FilePickerKeyFileButton_Click;
            //OpenDatabase.Click += OpenDatabase_Click;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            PWDatabaseDataSource.SetupDemoData();
            rootFrame.Navigate(typeof(EntryGroupListPage), new EntryGroupListPageViewModel(PWDatabaseDataSource.Root));
        }
    }
}
