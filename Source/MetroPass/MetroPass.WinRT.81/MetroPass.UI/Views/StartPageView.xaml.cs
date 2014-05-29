using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using MetroPass.UI.ViewModels;
using ReactiveUI;

namespace MetroPass.UI.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class StartPageView : Page, IViewFor<StartPageViewModel>
    {
        public StartPageView()
        {
            this.InitializeComponent();
            this.Loaded += StartPageView_Loaded;
        }

        void StartPageView_Loaded(object sender, RoutedEventArgs e)
        {
            this.WhenAnyValue(x => x.ViewModel.ItemSelected).Subscribe(x => BottomBar.IsOpen = ViewModel.ItemSelected);
            
        }


        public StartPageViewModel ViewModel
        {
            get { return DataContext as StartPageViewModel; }
            set { }
        }

        object IViewFor.ViewModel
        {
            get { return DataContext; }
            set { }
        }
    }
}
