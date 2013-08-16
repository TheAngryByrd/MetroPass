using System.Windows;
using Microsoft.Phone.Controls;
using ReactiveUI;

namespace MetroPass.WP8.UI
{
    public partial class MainPage : PhoneApplicationPage, IViewFor<AppBootstrapper>
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ViewModel = RxApp.DependencyResolver.GetService<AppBootstrapper>();
            this.OneWayBind(ViewModel, x => x.Router, x => x.Router.Router);
        }

        public AppBootstrapper ViewModel
        {
            get { return (AppBootstrapper)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AppBootstrapper), typeof(MainPage), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (AppBootstrapper)value; }
        }
    }
}