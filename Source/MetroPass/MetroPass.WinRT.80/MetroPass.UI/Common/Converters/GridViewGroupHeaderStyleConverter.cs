using System;
using MetroPass.UI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MetroPass.UI.Common.Converters
{
    public class GridViewGroupHeaderStyleConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(EntryGroupListViewModel), typeof(GridViewGroupHeaderStyleConverter), null);

        public EntryGroupListViewModel ViewModel
        {
            get { return (EntryGroupListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var uuid = value as String;

            if (uuid == ViewModel.Root.UUID)
            {
                return App.Current.Resources["GridViewHeaderRootStyle"];
            }
            return App.Current.Resources["GridViewHeaderNormalStyle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}